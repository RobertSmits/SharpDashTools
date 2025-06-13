param(
    [ValidateSet("vs2013", "vs2015")]
    [Parameter(Position = 0)]
    [string] $Target = "vs2015",
    [Parameter(Position = 1)]
    [string] $Version = "0.1.12",
    [Parameter(Position = 2)]
    [string] $AssemblyVersion = "0.1.12"
)

function Write-Diagnostic
{
    param(
        [Parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
        [string] $Message
    )

    Write-Host
    Write-Host $Message -ForegroundColor Green
    Write-Host
}

# https://github.com/jbake/Powershell_scripts/blob/master/Invoke-BatchFile.ps1
function Invoke-BatchFile
{
   param(
        [Parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
        [string]$Path,
        [Parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
        [string]$Parameters
   )

   $tempFile = [IO.Path]::GetTempFileName()

   cmd.exe /c " `"$Path`" $Parameters && set > `"$tempFile`" "

   Get-Content $tempFile | Foreach-Object {
       if ($_ -match "^(.*?)=(.*)$")
       {
           Set-Content "env:\$($matches[1])" $matches[2]
       }
   }

   Remove-Item $tempFile
}

function Die
{
    param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [string] $Message
    )

    Write-Host
    Write-Error $Message
    exit 1
}

function Warn
{
    param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [string] $Message
    )

    Write-Host
    Write-Host $Message -ForegroundColor Yellow
    Write-Host
}

function TernaryReturn
{
    param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [bool] $Yes,
        [Parameter(Position = 1, ValueFromPipeline = $true)]
        $Value,
        [Parameter(Position = 2, ValueFromPipeline = $true)]
        $Value2
    )

    if($Yes) {
        return $Value
    }

    $Value2
}

function Msvs
{
    param(
        [ValidateSet('v120', 'v140')]
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [string] $Toolchain,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [ValidateSet('Debug', 'Release')]
        [string] $Configuration,

        [Parameter(Position = 2, ValueFromPipeline = $true)]
        [ValidateSet('x86', 'x64', '"Mixed Platforms"', '"Any CPU"')]
        [string] $Platform
    )

    Write-Diagnostic "Targeting $Toolchain using configuration $Configuration on platform $Platform"

    $VisualStudioVersion = $null
    $VXXCommonTools = $null

    switch -Exact ($Toolchain) {
        'v120' {
            $MSBuildExe = join-path -path (Get-ItemProperty "HKLM:\software\Microsoft\MSBuild\ToolsVersions\12.0").MSBuildToolsPath -childpath "msbuild.exe"
            $MSBuildExe = $MSBuildExe -replace "Framework64", "Framework"
            $VisualStudioVersion = '12.0'
            $VXXCommonTools = Join-Path $env:VS120COMNTOOLS '..\..\vc'
        }
        'v140' {
            $MSBuildExe = join-path -path (Get-ItemProperty "HKLM:\software\Microsoft\MSBuild\ToolsVersions\14.0").MSBuildToolsPath -childpath "msbuild.exe"
            $MSBuildExe = $MSBuildExe -replace "Framework64", "Framework"
            $VisualStudioVersion = '14.0'
            $VXXCommonTools = Join-Path $env:VS140COMNTOOLS '..\..\vc'
        }
    }

    if ($VXXCommonTools -eq $null -or (-not (Test-Path($VXXCommonTools)))) {
        Die 'Error unable to find any visual studio environment'
    }

    $VCVarsAll = Join-Path $VXXCommonTools vcvarsall.bat
    if (-not (Test-Path $VCVarsAll)) {
        Die "Unable to find $VCVarsAll"
    }

    # Only configure build environment once
    if($env:CEFSHARP_BUILD_IS_BOOTSTRAPPED -eq $null) {
        Invoke-BatchFile $VCVarsAll $Platform
        $env:CEFSHARP_BUILD_IS_BOOTSTRAPPED = $true
    }

    $Arguments = @(
        "$slnFile",
        "/t:rebuild",
        "/p:VisualStudioVersion=$VisualStudioVersion",
        "/p:Configuration=$Configuration",
        "/p:Platform=$Platform",
        "/verbosity:normal"
    )

    $StartInfo = New-Object System.Diagnostics.ProcessStartInfo
    $StartInfo.FileName = $MSBuildExe
    $StartInfo.Arguments = $Arguments

    $StartInfo.EnvironmentVariables.Clear()

    Get-ChildItem -Path env:* | ForEach-Object {
        $StartInfo.EnvironmentVariables.Add($_.Name, $_.Value)
    }

    $StartInfo.UseShellExecute = $false
    $StartInfo.CreateNoWindow = $false
    $StartInfo.RedirectStandardError = $true
    $StartInfo.RedirectStandardOutput = $true

    $Process = New-Object System.Diagnostics.Process
    $Process.StartInfo = $startInfo
    $Process.Start()

    $stdout = $Process.StandardOutput.ReadToEnd()
    $stderr = $Process.StandardError.ReadToEnd()

    $Process.WaitForExit()

    if($Process.ExitCode -ne 0)
    {
        Write-Host "stdout: $stdout"
        Write-Host "stderr: $stderr"
        Die "Build failed"
    }
}

function VSX
{
    param(
        [ValidateSet('v120', 'v140')]
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [string] $Toolchain
    )

    if($Toolchain -eq 'v120' -and $env:VS120COMNTOOLS -eq $null) {
        Warn "Toolchain $Toolchain is not installed on your development machine, skipping build."
        Return
    }

    if($Toolchain -eq 'v140' -and $env:VS140COMNTOOLS -eq $null) {
        Warn "Toolchain $Toolchain is not installed on your development machine, skipping build."
        Return
    }

    Write-Diagnostic "Starting to build targeting toolchain $Toolchain"

    Msvs "$Toolchain" 'Release' '"Any CPU"'

    Write-Diagnostic "Finished build targeting toolchain $Toolchain"
}

function NugetPackageRestore
{
    $nuget = [IO.Path]::Combine($WorkingDir, 'NuGet', 'NuGet.exe')
    if(-not (Test-Path $nuget)) {
        Die "Please install nuget. More information available at: http://docs.nuget.org/docs/start-here/installing-nuget"
    }

    Write-Diagnostic "Restore Nuget Packages"

    # Restore packages
    . $nuget restore $slnFile
}

function Nupkg
{
    if (Test-Path Env:\APPVEYOR_PULL_REQUEST_NUMBER)
    {
        Write-Diagnostic "Pr Number: $env:APPVEYOR_PULL_REQUEST_NUMBER"
        Write-Diagnostic "Skipping Nupkg"
        return
    }

    $nuget = Join-Path $WorkingDir .\NuGet\NuGet.exe
    if(-not (Test-Path $nuget)) {
        Die "Please install nuget. More information available at: http://docs.nuget.org/docs/start-here/installing-nuget"
    }

    Write-Diagnostic "Building nuget package"

    # Build packages
    $nuspec = [IO.Path]::Combine($WorkingDir, 'NuGet', 'DashTools.csproj.nuspec')
    . $nuget pack "$nuspec" -NoPackageAnalysis -Symbol -Version $Version -OutputDirectory ([IO.Path]::Combine($WorkingDir, 'NuGet'))

    # Invoke `AfterBuild` script if available (ie. upload packages to myget)
    if(-not (Test-Path $WorkingDir\AfterBuild.ps1)) {
        return
    }

    . $WorkingDir\AfterBuild.ps1 -Version $Version
}

function DownloadNuget()
{
	$nugetDir = [IO.Path]::Combine($WorkingDir, 'NuGet')
    $nuget = [IO.Path]::Combine($nugetDir, 'NuGet.exe')
    if(-not (Test-Path $nuget))
    {
		if (-not (Test-Path $nugetDir))
		{
			New-Item -Path $nugetDir -ItemType Directory
		}
        $client = New-Object System.Net.WebClient;
        $client.DownloadFile('http://nuget.org/nuget.exe', $nuget);
    }
}

function WriteAssemblyVersion
{
    param()

    $Filename = [IO.Path]::Combine($WorkingDir, 'DashTools', 'Properties', 'AssemblyInfo.cs')
    $Regex = 'public const string AssemblyVersion = "(.*)"';

    $AssemblyInfo = Get-Content $Filename
    $NewString = $AssemblyInfo -replace $Regex, "public const string AssemblyVersion = ""$AssemblyVersion"""

    $NewString | Set-Content $Filename -Encoding UTF8
}

$WorkingDir = split-path -parent $MyInvocation.MyCommand.Definition

$slnFile = [IO.Path]::Combine($WorkingDir, 'Qoollo.MpegDash.slnx')

DownloadNuget

NugetPackageRestore

WriteAssemblyVersion

switch -Exact ($Target)
{
    "nupkg-only"
    {
        Nupkg
    }
    "vs2013"
    {
        VSX v120
        Nupkg
    }
    "vs2015"
    {
        VSX v140
        Nupkg
    }
}
