using Qoollo.MpegDash.Mpd;
using Xunit;

namespace Qoollo.MpegDash.Tests;

public class MpdDownloaderTests : IClassFixture<MpdFixture>
{
    private readonly MediaPresentationDescription _mpd;

    public MpdDownloaderTests(MpdFixture mpdFixture)
    {
        _mpd = mpdFixture.Mpd;
    }

    [Fact]
    public void Download()
    {
        //  arrange
        //var dir = new DirectoryInfo("envivio");
        //if (dir.Exists)
        //    dir.Delete(recursive: true);
        //var downloader = new MpdDownloader(new Uri("http://dash.edgesuite.net/envivio/EnvivioDash3/manifest.mpd"), dir.FullName);

        ////  act
        //var task = downloader.Download();
        //task.Wait();
        //var actual = task.Result;

        //  assert
    }
}
