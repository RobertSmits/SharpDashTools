namespace Qoollo.MpegDash;

public class Mp4File
{
    private readonly string _path;

    public Mp4File(string path)
    {
        _path = path;
    }

    public string Path => _path;
}
