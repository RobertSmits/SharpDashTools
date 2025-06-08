using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class SegmentBase : MpdElement
{
    internal SegmentBase(XElement node)
        : base(node) { }

    public uint? Timescale => _node.ParseOptionalUint("timescale");

    public ulong? PresentationTimeOffset => _node.ParseOptionalUlong("presentationTimeOffset");

    public string IndexRange => _node.ParseMandatoryString("indexRange");

    public bool IndexRangeExact => _node.ParseOptionalBool("indexRangeExact", false);

    public double? AvailabilityTimeOffset => _node.ParseOptionalDouble("availabilityTimeOffset");

    public bool AvailabilityTimeComplete => _node.ParseOptionalBool("availabilityTimeComplete", false);
}
