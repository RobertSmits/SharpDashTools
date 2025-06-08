using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdSegmentList : MpdElement
{
    private readonly Lazy<MpdInitialization?> _initialization;
    private readonly Lazy<IEnumerable<MpdSegmentUrl>> _segmentUrls;

    internal MpdSegmentList(XElement node)
        : base(node)
    {
        _initialization = new Lazy<MpdInitialization?>(ParseInitialization);
        _segmentUrls = new Lazy<IEnumerable<MpdSegmentUrl>>(ParseSegmentUrls);
    }

    public uint? Timescale => _helper.ParseOptionalUint("timescale");

    public uint? Duration => _helper.ParseOptionalUint("duration");

    public MpdInitialization? Initialization => _initialization.Value;

    public IEnumerable<MpdSegmentUrl> SegmentUrls => _segmentUrls.Value;

    private MpdInitialization? ParseInitialization()
    {
        return _node
            .Elements()
            .Where(n => n.Name.LocalName == "Initialization")
            .Select(n => new MpdInitialization(n))
            .FirstOrDefault();
    }

    private IEnumerable<MpdSegmentUrl> ParseSegmentUrls()
    {
        return _node.Elements().Where(n => n.Name.LocalName == "SegmentURL").Select(n => new MpdSegmentUrl(n));
    }
}
