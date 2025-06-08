using System.Net;
using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MediaPresentationDescription : IDisposable
{
    private readonly Stream stream;
    private readonly bool streamIsOwned;
    private readonly Lazy<XElement> mpdTag;
    private readonly Lazy<XmlAttributeParseHelper> helper;
    private readonly Lazy<DateTimeOffset> fetchTime;
    private readonly Lazy<string?> baseURL;
    private readonly Lazy<IEnumerable<MpdPeriod>> periods;

    public MediaPresentationDescription(Stream mpdStream)
        : this(mpdStream, false)
    {
    }

    public MediaPresentationDescription(string mpdFilePath)
        : this(File.OpenRead(mpdFilePath), true)
    {
    }

    private MediaPresentationDescription(Stream mpdStream, bool streamIsOwned)
    {
        stream = mpdStream;
        this.streamIsOwned = streamIsOwned;

        baseURL = new Lazy<string?>(ParseBaseURL);
        mpdTag = new Lazy<XElement>(ReadMpdTag);
        fetchTime = new Lazy<DateTimeOffset>(() => { var v = mpdTag.Value; return DateTimeOffset.Now; });
        helper = new Lazy<XmlAttributeParseHelper>(() => new XmlAttributeParseHelper(mpdTag.Value));
        periods = new Lazy<IEnumerable<MpdPeriod>>(ParsePeriods);
    }

    public static MediaPresentationDescription FromUrl(Uri url, string? saveFilePath = null)
    {
        if (saveFilePath == null)
            saveFilePath = Path.GetTempFileName();
        using (var client = new WebClient())
        {
            client.DownloadFile(url, saveFilePath);
            return new MediaPresentationDescription(saveFilePath);
        }
    }

    public DateTimeOffset FetchTime => fetchTime.Value;

    public string? Id => mpdTag.Value.Attribute("id")?.Value;

    public string Type => mpdTag.Value.Attribute("type")?.Value ?? "static";

    public string? Profiles => helper.Value.ParseOptionalString("profiles");

    public DateTimeOffset? AvailabilityStartTime => helper.Value.ParseDateTimeOffset("availabilityStartTime", Type == "dynamic");

    public DateTimeOffset? PublishTime => helper.Value.ParseOptionalDateTimeOffset("publishTime");

    public DateTimeOffset? AvailabilityEndTime => helper.Value.ParseOptionalDateTimeOffset("availabilityEndTime");

    public TimeSpan? MediaPresentationDuration => helper.Value.ParseOptionalTimeSpan("mediaPresentationDuration");

    public TimeSpan? MinimumUpdatePeriod => helper.Value.ParseOptionalTimeSpan("minimumUpdatePeriod");

    public TimeSpan MinBufferTime => helper.Value.ParseMandatoryTimeSpan("minBufferTime");

    public TimeSpan? TimeShiftBufferDepth => helper.Value.ParseOptionalTimeSpan("timeShiftBufferDepth");

    public TimeSpan? SuggestedPresentationDelay => helper.Value.ParseOptionalTimeSpan("suggestedPresentationDelay");

    public TimeSpan? MaxSegmentDuration => helper.Value.ParseOptionalTimeSpan("maxSegmentDuration");

    public TimeSpan? MaxSubsegmentDuration => helper.Value.ParseOptionalTimeSpan("maxSubsegmentDuration");

    public string? BaseURL => baseURL.Value;

    private string? ParseBaseURL()
    {
        return mpdTag.Value.Elements()
            .Where(n => n.Name.LocalName == "BaseURL")
            .Select(n => n.Value)
            .FirstOrDefault();
    }

    public IEnumerable<MpdPeriod> Periods => periods.Value;

    public void Save(string filename)
    {
        using (var fileStream = File.OpenWrite(filename))
        {
            stream.CopyTo(fileStream);
            stream.Seek(0, SeekOrigin.Begin);
        }
    }

    private XElement ReadMpdTag()
    {
        var doc = XDocument.Load(stream);
        return doc.Root ?? throw new Exception();

        //using (var reader = XmlReader.Create(stream))
        //{
        //    stream.Seek(0, SeekOrigin.Begin);
        //    reader.ReadToFollowing("MPD");
        //    return XNode.ReadFrom(reader) as XElement;
        //}
    }

    private IEnumerable<MpdPeriod> ParsePeriods()
    {
        return mpdTag.Value.Elements()
            .Where(n => n.Name.LocalName == "Period")
            .Select(n => new MpdPeriod(n));
    }

    #region IDisposable

    public void Dispose()
    {
        if (streamIsOwned)
            stream.Dispose();
    }

    #endregion
}
