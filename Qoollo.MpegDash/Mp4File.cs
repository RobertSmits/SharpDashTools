namespace Qoollo.MpegDash;

public class Mp4File
{
    private readonly string path;
    
    public Mp4File(string path)
    {
        this.path = path;
    }

    public string Path => path;
}
