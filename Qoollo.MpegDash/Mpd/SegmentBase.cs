using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class SegmentBase : MpdElement
{
    internal SegmentBase(XElement node)
        : base(node)
    {
    }

    public uint? Timescale => _helper.ParseOptionalUint("timescale");

    public ulong? PresentationTimeOffset => _helper.ParseOptionalUlong("presentationTimeOffset");

    public string IndexRange => _helper.ParseMandatoryString("indexRange");

    public bool IndexRangeExact => _helper.ParseOptionalBool("indexRangeExact", false);

    public double? AvailabilityTimeOffset => _helper.ParseOptionalDouble("availabilityTimeOffset");

    public bool AvailabilityTimeComplete => _helper.ParseOptionalBool("availabilityTimeComplete", false);
}
