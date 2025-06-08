using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class SegmentBase : MpdElement
{
    internal SegmentBase(XElement node)
        : base(node)
    {
    }

    public uint? Timescale => helper.ParseOptionalUint("timescale");

    public ulong? PresentationTimeOffset => helper.ParseOptionalUlong("presentationTimeOffset");

    public string IndexRange => helper.ParseMandatoryString("indexRange");

    public bool IndexRangeExact => helper.ParseOptionalBool("indexRangeExact", false);

    public double? AvailabilityTimeOffset => helper.ParseOptionalDouble("availabilityTimeOffset");

    public bool AvailabilityTimeComplete => helper.ParseOptionalBool("availabilityTimeComplete", false);
}
