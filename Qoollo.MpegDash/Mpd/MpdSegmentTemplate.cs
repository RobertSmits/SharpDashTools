using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

/// <summary>
/// Specifies Segment Template information.
/// </summary>
public class MpdSegmentTemplate : MultipleSegmentBase
{
    private readonly SegmentTimeline? _segmentTimeline;

    internal MpdSegmentTemplate(XElement node)
        : base(node)
    {
        _segmentTimeline = ParseSegmentTimeline();
    }

    /// <summary>
    /// Optional
    ///
    /// Specifies the template to create the Media Segment List.
    /// </summary>
    public string? Media => _helper.ParseOptionalString("media");

    /// <summary>
    /// Optional
    ///
    /// Specifies the template to create the Index Segment List.
    /// If neither the $Number$ nor the $Time$ identifier is included,
    /// this provides the URL to a Representation Index.
    /// </summary>
    public string? Index => _helper.ParseOptionalString("index");

    /// <summary>
    /// Optional
    ///
    /// Specifies the template to create the Initialization Segment.
    /// Neither $Number$ nor the $Time$ identifier shall be included.
    /// </summary>
    public string? Initialization => _helper.ParseOptionalString("initialization");

    /// <summary>
    /// Optional
    ///
    /// Specifies the template to create the Bitstream Switching Segment.
    /// Neither $Number$ nor the $Time$ identifier shall be included.
    /// </summary>
    public bool BitstreamSwitching => _helper.ParseOptionalBool("bitstreamSwitching", false);

    public SegmentTimeline? SegmentTimeline => _segmentTimeline;

    private SegmentTimeline? ParseSegmentTimeline()
    {
        return _node
            .Elements()
            .Where(n => n.Name.LocalName == "SegmentTimeline")
            .Select(n => new SegmentTimeline(n))
            .FirstOrDefault();
    }
}
