using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdSegmentList : MpdElement
{
    private readonly Lazy<MpdInitialization?> initialization;
    private readonly Lazy<IEnumerable<MpdSegmentUrl>> segmentUrls;

    internal MpdSegmentList(XElement node)
        : base(node)
    {
        initialization = new Lazy<MpdInitialization?>(ParseInitialization);
        segmentUrls = new Lazy<IEnumerable<MpdSegmentUrl>>(ParseSegmentUrls);
    }

    public uint? Timescale => helper.ParseOptionalUint("timescale");

    public uint? Duration => helper.ParseOptionalUint("duration");

    public MpdInitialization? Initialization => initialization.Value;

    public IEnumerable<MpdSegmentUrl> SegmentUrls => segmentUrls.Value;

    private MpdInitialization? ParseInitialization()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "Initialization")
            .Select(n => new MpdInitialization(n))
            .FirstOrDefault();
    }

    private IEnumerable<MpdSegmentUrl> ParseSegmentUrls()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "SegmentURL")
            .Select(n => new MpdSegmentUrl(n));
    }
}
