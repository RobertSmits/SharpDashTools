using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MediaPresentationDescription : IDisposable
{
    private readonly Stream _stream;
    private readonly bool _streamIsOwned;
    private readonly Lazy<XElement> _mpdTag;
    private readonly Lazy<DateTimeOffset> _fetchTime;
    private readonly Lazy<string?> _baseURL;
    private readonly Lazy<IEnumerable<MpdPeriod>> _periods;

    public MediaPresentationDescription(Stream mpdStream)
        : this(mpdStream, false) { }

    public MediaPresentationDescription(string mpdFilePath)
        : this(File.OpenRead(mpdFilePath), true) { }

    private MediaPresentationDescription(Stream mpdStream, bool streamIsOwned)
    {
        _stream = mpdStream;
        _streamIsOwned = streamIsOwned;

        _baseURL = new Lazy<string?>(ParseBaseURL);
        _mpdTag = new Lazy<XElement>(ReadMpdTag);
        _fetchTime = new Lazy<DateTimeOffset>(() =>
        {
            var v = _mpdTag.Value;
            return DateTimeOffset.Now;
        });
        _periods = new Lazy<IEnumerable<MpdPeriod>>(ParsePeriods);
    }

    public static async Task<MediaPresentationDescription> FromUrlAsync(
        Uri url,
        string? saveFilePath = null,
        CancellationToken cancellationToken = default
    )
    {
        saveFilePath ??= Path.GetTempFileName();
        using var client = new HttpClient();
        using var data = await client.GetStreamAsync(url, cancellationToken);
        await using var fs = File.Create(saveFilePath);
        await data.CopyToAsync(fs, cancellationToken);
        return new MediaPresentationDescription(saveFilePath);
    }

    public DateTimeOffset FetchTime => _fetchTime.Value;

    public string? Id => _mpdTag.Value.Attribute("id")?.Value;

    public string Type => _mpdTag.Value.Attribute("type")?.Value ?? "static";

    public string? Profiles => _mpdTag.Value.ParseOptionalString("profiles");

    public DateTimeOffset? AvailabilityStartTime =>
        _mpdTag.Value.ParseDateTimeOffset("availabilityStartTime", Type == "dynamic");

    public DateTimeOffset? PublishTime => _mpdTag.Value.ParseOptionalDateTimeOffset("publishTime");

    public DateTimeOffset? AvailabilityEndTime => _mpdTag.Value.ParseOptionalDateTimeOffset("availabilityEndTime");

    public TimeSpan? MediaPresentationDuration => _mpdTag.Value.ParseOptionalTimeSpan("mediaPresentationDuration");

    public TimeSpan? MinimumUpdatePeriod => _mpdTag.Value.ParseOptionalTimeSpan("minimumUpdatePeriod");

    public TimeSpan MinBufferTime => _mpdTag.Value.ParseMandatoryTimeSpan("minBufferTime");

    public TimeSpan? TimeShiftBufferDepth => _mpdTag.Value.ParseOptionalTimeSpan("timeShiftBufferDepth");

    public TimeSpan? SuggestedPresentationDelay => _mpdTag.Value.ParseOptionalTimeSpan("suggestedPresentationDelay");

    public TimeSpan? MaxSegmentDuration => _mpdTag.Value.ParseOptionalTimeSpan("maxSegmentDuration");

    public TimeSpan? MaxSubsegmentDuration => _mpdTag.Value.ParseOptionalTimeSpan("maxSubsegmentDuration");

    public string? BaseURL => _baseURL.Value;

    private string? ParseBaseURL()
    {
        return _mpdTag.Value.Elements().Where(n => n.Name.LocalName == "BaseURL").Select(n => n.Value).FirstOrDefault();
    }

    public IEnumerable<MpdPeriod> Periods => _periods.Value;

    public void Save(string filename)
    {
        using var fileStream = File.OpenWrite(filename);
        _stream.CopyTo(fileStream);
        _stream.Seek(0, SeekOrigin.Begin);
    }

    private XElement ReadMpdTag()
    {
        var doc = XDocument.Load(_stream);
        return doc.Root ?? throw new Exception();

        //using var reader = XmlReader.Create(stream);
        //stream.Seek(0, SeekOrigin.Begin);
        //reader.ReadToFollowing("MPD");
        //return XNode.ReadFrom(reader) as XElement;
    }

    private IEnumerable<MpdPeriod> ParsePeriods()
    {
        return _mpdTag.Value.Elements().Where(n => n.Name.LocalName == "Period").Select(n => new MpdPeriod(n));
    }

    #region IDisposable

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (_streamIsOwned)
            _stream.Dispose();
    }

    #endregion
}
