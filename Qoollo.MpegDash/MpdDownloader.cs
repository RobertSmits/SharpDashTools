using System.Text;
using Qoollo.MpegDash.Mpd;

namespace Qoollo.MpegDash;

public class MpdDownloader : IDisposable
{
    private readonly Uri _mpdUrl;
    private readonly string _destinationDir;
    private readonly Lazy<string> _mpdFileName;
    private readonly Lazy<MediaPresentationDescription> _mpd;
    private readonly Lazy<MpdWalker> _walker;
    private readonly int _downloadConcurrency;

    public MpdDownloader(Uri mpdUrl, string destinationDir, int downloadConcurrency = 2)
    {
        if (downloadConcurrency < 1)
            throw new ArgumentException("downloadConcurrency cannot be less than 1.", nameof(downloadConcurrency));

        _mpdUrl = mpdUrl ?? throw new ArgumentNullException(nameof(mpdUrl));
        _destinationDir = destinationDir ?? throw new ArgumentNullException(nameof(destinationDir));
        _downloadConcurrency = downloadConcurrency;

        _mpdFileName = new Lazy<string>(GetMpdFileName);
        _mpd = new Lazy<MediaPresentationDescription>(DownloadMpdAsync().Result);
        _walker = new Lazy<MpdWalker>(CreateMpdWalker);
    }

    public IEnumerable<Track> GetTracksFor(TrackContentType type)
    {
        return _walker.Value.GetTracksFor(type);
    }

    public Task<IEnumerable<Mp4File>> Download(TrackRepresentation trackRepresentation)
    {
        return Task.Factory.StartNew(() =>
            DownloadTrackRepresentation(trackRepresentation, TimeSpan.Zero, TimeSpan.MaxValue)
        );
    }

    public Task<IEnumerable<Mp4File>> Download(TrackRepresentation trackRepresentation, TimeSpan from, TimeSpan to)
    {
        return Task.Factory.StartNew(() => DownloadTrackRepresentation(trackRepresentation, from, to));
    }

    public FileInfo CombineChunksFast(IEnumerable<Mp4File> chunks, Action<string> ffmpegRunner)
    {
        if (!chunks.Any())
            throw new ArgumentException("Chunks must not be empty.", nameof(chunks));
        var firstChunkPath = chunks.First().Path;
        var dir = Path.GetDirectoryName(firstChunkPath) ?? string.Empty;
        var concatFile = Path.Combine(dir, string.Format("{0:yyyyMMddHHmmssfffffff}_concat.mp4", DateTime.Now));
        var outFile = concatFile.Replace("_concat.mp4", "_video.mp4");

        if (File.Exists(concatFile))
            File.Delete(concatFile);
        if (File.Exists(outFile))
            File.Delete(outFile);

        var initFile = chunks.OfType<Mp4InitFile>().First();
        var files = chunks.ToList();
        files.Remove(initFile);
        files.Insert(0, initFile);

        ConcatFiles(files.Select(f => f.Path), concatFile);

        ffmpegRunner(
            string.Format(
                "-i \"concat:{0}\" -c copy {1}",
                ConvertPathForFfmpeg(concatFile),
                ConvertPathForFfmpeg(outFile)
            )
        );

        return new FileInfo(outFile);
    }

    private static void ConcatFiles(IEnumerable<string> files, string outFile)
    {
        using var stream = File.OpenWrite(outFile);
        foreach (var f in files)
        {
            var bytes = File.ReadAllBytes(f);
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    public FileInfo CombineChunksFastOld(
        IEnumerable<Mp4File> chunks,
        Action<string> ffmpegRunner,
        int maxCmdLength = 32672
    )
    {
        if (!chunks.Any())
            throw new ArgumentException("Chunks must not be empty.", nameof(chunks));
        var firstChunkPath = chunks.First().Path;
        var dir = Path.GetDirectoryName(firstChunkPath) ?? string.Empty;
        string outputFile = Path.Combine(dir, string.Format("{0:yyyyMMddHHmmssfffffff}_combined.mp4", DateTime.Now));

        var cmdBuilder = new StringBuilder(maxCmdLength);
        var initFile = chunks.OfType<Mp4InitFile>().First();
        var files = chunks.Except([initFile]).ToList();

        cmdBuilder.AppendFormat("-i \"concat:{0}", ConvertPathForFfmpeg(initFile.Path));
        if (File.Exists(outputFile))
            File.Delete(outputFile);
        string cmdEnd = "\" -c copy " + ConvertPathForFfmpeg(outputFile);

        bool overflow = false;
        while (!overflow && files.Count != 0)
        {
            string toAppend = "|" + ConvertPathForFfmpeg(files[0].Path);
            if (cmdBuilder.Length + toAppend.Length + cmdEnd.Length > maxCmdLength)
                overflow = true;
            else
            {
                cmdBuilder.Append(toAppend);
                files.RemoveAt(0);
            }
        }
        cmdBuilder.Append(cmdEnd);

        ffmpegRunner(cmdBuilder.ToString());
        cmdBuilder.Clear();

        FileInfo res;
        if (files.Count != 0)
        {
            files.Insert(0, new Mp4InitFile(outputFile));
            res = CombineChunksFastOld(files, ffmpegRunner, maxCmdLength);
        }
        else
            res = new FileInfo(outputFile);

        return res;
    }

    public FileInfo CombineChunks(IEnumerable<Mp4File> chunks, Action<string> ffmpegRunner)
    {
        if (!chunks.Any())
            throw new ArgumentException("Chunks must not be empty.", nameof(chunks));
        var firstChunkPath = chunks.First().Path;
        var dir = Path.GetDirectoryName(firstChunkPath) ?? string.Empty;

        chunks = ProcessChunks(chunks);

        string tempFile = Path.Combine(dir, string.Format("{0:yyyyMMddHHmmss}_temp.mp4", DateTime.Now));
        foreach (var c in chunks)
        {
            ffmpegRunner(
                string.Format(
                    @"-i ""{0}"" -filter:v ""setpts=PTS-STARTPTS"" -f mp4 ""{1}""",
                    ConvertPathForFfmpeg(c.Path),
                    ConvertPathForFfmpeg(tempFile)
                )
            );
            File.Delete(c.Path);
            File.Move(tempFile, c.Path);
        }
        File.Delete(tempFile);

        string filesListFile = Path.Combine(dir, string.Format("{0:yyyyMMddHHmmss}_list.txt", DateTime.Now));
        File.WriteAllText(
            filesListFile,
            string.Join("", chunks.Select(c => string.Format("file '{0}'\r\n", Path.GetFileName(c.Path))))
        );
        string outFile = Path.Combine(dir, string.Format("{0:yyyyMMddHHmmss}_combined.mp4", DateTime.Now));
        if (File.Exists(outFile))
            File.Delete(outFile);
        ffmpegRunner(
            string.Format(
                @"-f concat -i ""{0}"" -c copy ""{1}""",
                ConvertPathForFfmpeg(filesListFile),
                ConvertPathForFfmpeg(outFile)
            )
        );
        File.Delete(filesListFile);

        return new FileInfo(outFile);
    }

    private static string ConvertPathForFfmpeg(string path)
    {
        return path.Replace("\\", "/");
    }

    private IEnumerable<Mp4File> DownloadTrackRepresentation(
        TrackRepresentation trackRepresentation,
        TimeSpan from,
        TimeSpan to,
        int concurrency = 2
    )
    {
        string initFile;
        var files = new List<string>();

        var task = DownloadFragment(trackRepresentation.InitFragmentPath);
        task.Wait(TimeSpan.FromMinutes(5));
        if (DownloadTaskSucceded(task))
        {
            initFile = task.Result;

            bool complete = false;
            int downloadedCount = 0;
            while (!complete)
            {
                var tasks = trackRepresentation
                    .GetFragmentsPaths(from, to)
                    .Skip(downloadedCount)
                    .Take(concurrency)
                    .Select(p => DownloadFragment(p))
                    .ToList();
                tasks.ForEach(t => t.Wait(TimeSpan.FromMinutes(5)));
                var succeeded = tasks.Where(t => DownloadTaskSucceded(t)).ToList();
                succeeded.ForEach(t => files.Add(t.Result));

                downloadedCount += succeeded.Count;
                complete = tasks.Count == 0 || succeeded.Count != tasks.Count;
            }

            //var chunks = ProcessChunks(initFile, files);
            var chunks = files.Select(f => new Mp4File(f)).ToList();
            chunks.Insert(0, new Mp4InitFile(initFile));

            //DeleteAllFilesExcept(outputFile, destinationDir);

            return chunks;
        }
        else
            throw new Exception("Failed to download init file");
    }

    private static IEnumerable<Mp4File> ProcessChunks(IEnumerable<Mp4File> files)
    {
        var initFile = files.OfType<Mp4InitFile>().FirstOrDefault();
        if (initFile is null)
            return [.. files];

        var res = new List<Mp4File>();
        var initFileBytes = File.ReadAllBytes(initFile.Path);
        foreach (var f in files)
        {
            var bytes = File.ReadAllBytes(f.Path);
            if (!bytes.StartsWith(initFileBytes))
            {
                bytes = initFileBytes.Concat(bytes).ToArray();
                File.WriteAllBytes(f.Path, bytes);
                res.Add(f);
            }
        }

        return res;

        //string outputFile = Path.Combine(destinationDir, DateTime.Now.ToString("yyyyMMddHHmmss") + "_video.mp4");
        //using var stream = File.OpenWrite(outputFile);
        //using var writer = new BinaryWriter(stream);
        //
        //foreach (var f in files.Skip(1))
        //{
        //    var bytes = File.ReadAllBytes(f);
        //    var mdatBytes = Encoding.ASCII.GetBytes("mdat");
        //    int offset = FindAtomOffset(bytes, mdatBytes);
        //    //if (offset >= 0)
        //    //    writer.Write(bytes, offset - mdatBytes.Length, bytes.Length - offset + mdatBytes.Length);
        //    writer.Write(bytes);
        //}
    }

    private static int FindAtomOffset(byte[] chunkBytes, byte[] atomBytes)
    {
        int mdatOffset = -1;
        for (int i = 0; i < chunkBytes.Length - atomBytes.Length && mdatOffset < 0; i++)
        {
            int matchCount = 0;
            for (int j = 0; j < atomBytes.Length; j++)
            {
                if (chunkBytes[i + j] == atomBytes[j])
                    matchCount++;
            }
            if (matchCount == atomBytes.Length)
                mdatOffset = i;
        }
        return mdatOffset;
    }

    private void DeleteAllFilesExcept(string outputFile, string destinationDir)
    {
        outputFile = Path.GetFullPath(outputFile);
        var files = Directory.GetFiles(destinationDir);
        _mpd.Value.Dispose();
        foreach (var f in files)
        {
            string file = Path.GetFullPath(f);
            if (outputFile != file)
                File.Delete(file);
        }
    }

    public Task<string> Download()
    {
        var tasks = new List<Task>();

        foreach (var period in _mpd.Value.Periods)
        {
            foreach (var adaptationSet in period.AdaptationSets)
            {
                foreach (var representation in adaptationSet.Representations)
                {
                    tasks.Add(DownloadAllFragments(adaptationSet, representation));
                }
            }
        }

        return Task.Factory.ContinueWhenAll(
            tasks.ToArray(),
            completed =>
                CombineFragments(
                    _mpd.Value,
                    _mpdFileName.Value,
                    Path.Combine(Path.GetDirectoryName(_mpdFileName.Value) ?? string.Empty, "video.mp4")
                )
        );
    }

    private Task DownloadAllFragments(MpdAdaptationSet adaptationSet, MpdRepresentation representation)
    {
        return Task.Factory.StartNew(() => DownloadFragmentsUntilFirstFailure(adaptationSet, representation));
    }

    private static string CombineFragments(MediaPresentationDescription mpd, string mpdFilePath, string outputFilePath)
    {
        var walker = new MpdWalker(mpd);
        var track = walker.GetTracksFor(TrackContentType.Video).First();
        var trackRepresentation = track.TrackRepresentations.OrderByDescending(r => r.Bandwidth).First();

        var dir = Path.GetDirectoryName(mpdFilePath) ?? string.Empty;
        using var stream = File.OpenWrite(outputFilePath);
        using var writer = new BinaryWriter(stream);
        string fragmentPath = Path.Combine(dir, trackRepresentation.InitFragmentPath);
        writer.Write(File.ReadAllBytes(fragmentPath));

        foreach (var path in trackRepresentation.FragmentsPaths)
        {
            fragmentPath = Path.Combine(dir, path);
            if (!File.Exists(fragmentPath))
                break;
            writer.Write(File.ReadAllBytes(fragmentPath));
        }

        return outputFilePath;
    }

    private void DownloadFragmentsUntilFirstFailure(MpdAdaptationSet adaptationSet, MpdRepresentation representation)
    {
        var task = DownloadRepresentationInitFragment(adaptationSet, representation);

        if (DownloadTaskSucceded(task))
        {
            int i = 1;
            do
            {
                task = DownloadRepresentationFragment(adaptationSet, representation, i);
                i++;
            } while (DownloadTaskSucceded(task));
        }
    }

    private Task<string> DownloadRepresentationInitFragment(
        MpdAdaptationSet adaptationSet,
        MpdRepresentation representation
    )
    {
        if (adaptationSet.SegmentTemplate?.Initialization is null || representation.Id is null)
            throw new InvalidOperationException("SegmentTemplate.Initialization or Representation.Id is null");

        string initUrl = adaptationSet.SegmentTemplate.Initialization.Replace("$RepresentationID$", representation.Id);
        var task = DownloadFragment(initUrl);
        task.Wait(TimeSpan.FromMinutes(5));

        return task;
    }

    private Task<string> DownloadRepresentationFragment(
        MpdAdaptationSet adaptationSet,
        MpdRepresentation representation,
        int index
    )
    {
        if (adaptationSet.SegmentTemplate?.Media is null || representation.Id is null)
            throw new InvalidOperationException("SegmentTemplate.Media or Representation.Id is null");

        string fragmentUrl = adaptationSet
            .SegmentTemplate.Media.Replace("$RepresentationID$", representation.Id)
            .Replace("$Number$", index.ToString());

        var task = DownloadFragment(fragmentUrl);
        task.Wait(TimeSpan.FromMinutes(5));

        return task;
    }

    private Task<string> DownloadFragment(string fragmentUrl)
    {
        var url =
            IsAbsoluteUrl(fragmentUrl) ? new Uri(fragmentUrl)
            : _mpd.Value.BaseURL is not null ? new Uri(_mpd.Value.BaseURL + fragmentUrl)
            : new Uri(_mpdUrl, fragmentUrl);

        string destPath = Path.Combine(_destinationDir, GetFileNameForFragmentUrl(fragmentUrl));

        int i = 0;
        while (File.Exists(destPath))
        {
            i++;
            destPath = Path.Combine(
                Path.GetDirectoryName(destPath)!,
                Path.ChangeExtension(
                    (Path.GetFileNameWithoutExtension(destPath) + "_" + i),
                    Path.GetExtension(destPath)
                )
            );
        }

        // create directory recursive
        Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);

        return Task
            .Factory.StartNew(async () =>
            {
                using var client = new HttpClient();
                using var data = await client.GetStreamAsync(url);
                await using var fs = File.Create(destPath);
                await data.CopyToAsync(fs);
                return destPath;
            })
            .Unwrap();
    }

    private static bool DownloadTaskSucceded(Task<string> task)
    {
        return task.IsCompleted && !task.IsFaulted && !string.IsNullOrWhiteSpace(task.Result);
    }

    private string GetMpdFileName()
    {
        string mpdFileName = _mpdUrl.AbsolutePath;
        if (mpdFileName.Contains('/'))
            mpdFileName = mpdFileName[(mpdFileName.LastIndexOf('/') + 1)..];
        string mpdPath = Path.Combine(_destinationDir, mpdFileName);

        return mpdPath;
    }

    private Task<MediaPresentationDescription> DownloadMpdAsync()
    {
        if (!Directory.Exists(_destinationDir))
            Directory.CreateDirectory(_destinationDir);
        else
            Directory.GetFiles(_destinationDir).ToList().ForEach(f => File.Delete(f));

        return MediaPresentationDescription.FromUrlAsync(_mpdUrl, _mpdFileName.Value);
    }

    private MpdWalker CreateMpdWalker()
    {
        return new MpdWalker(_mpd.Value);
    }

    private static string GetFileNameForFragmentUrl(string url)
    {
        string fileName = url;
        if (IsAbsoluteUrl(url))
        {
            fileName = new Uri(url).AbsolutePath;
            if (fileName.Contains('/'))
                fileName = fileName[(fileName.LastIndexOf('/') + 1)..];
        }

        int queryStartIndex = fileName.IndexOf('?');
        if (queryStartIndex >= 0)
            fileName = fileName[..queryStartIndex];

        string extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
            fileName = Path.ChangeExtension(fileName, "mp4");

        fileName = ReplaceIllegalCharsInFileName(fileName);

        return fileName;
    }

    private static string ReplaceIllegalCharsInFileName(string fileName)
    {
        var illegalChars = new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
        foreach (var ch in illegalChars)
        {
            fileName = fileName.Replace(ch, '_');
        }
        return fileName;
    }

    private static bool IsAbsoluteUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, result: out _);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (_mpd.IsValueCreated)
            _mpd.Value.Dispose();
    }
}
