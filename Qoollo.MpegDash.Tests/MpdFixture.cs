using Qoollo.MpegDash.Mpd;

namespace Qoollo.MpegDash.Tests;

public class MpdFixture : IDisposable
{
    public MpdFixture()
    {
        Stream = File.OpenRead("envivio.mpd");
        Mpd = new MediaPresentationDescription(Stream);
    }

    public void Dispose()
    {
        Stream.Dispose();
    }

    public Stream Stream { get; private set; }

    public MediaPresentationDescription Mpd { get; private set; }
}
