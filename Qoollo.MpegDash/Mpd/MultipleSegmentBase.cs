using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

/// <summary>
/// Specifies multiple Segment base information.
/// </summary>
public abstract class MultipleSegmentBase : SegmentBase
{
    internal MultipleSegmentBase(XElement node)
        : base(node) { }

    public uint? Duration => _node.ParseOptionalUint("duration");

    public uint? StartNumber => _node.ParseOptionalUint("startNumber");
}
