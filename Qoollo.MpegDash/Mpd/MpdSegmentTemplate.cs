using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

/// <summary>
/// Specifies Segment Template information.
/// </summary>
public class MpdSegmentTemplate : MultipleSegmentBase
{
    private readonly SegmentTimeline? segmentTimeline;

    internal MpdSegmentTemplate(XElement node)
        : base(node)
    {
        segmentTimeline = ParseSegmentTimeline();
    }

    /// <summary>
    /// Optional
    ///
    /// Specifies the template to create the Media Segment List.
    /// </summary>
    public string? Media => helper.ParseOptionalString("media");

    /// <summary>
    /// Optional
    ///
    /// Specifies the template to create the Index Segment List.
    /// If neither the $Number$ nor the $Time$ identifier is included,
    /// this provides the URL to a Representation Index.
    /// </summary>
    public string? Index => helper.ParseOptionalString("index");

    /// <summary>
    /// Optional
    ///
    /// Specifies the template to create the Initialization Segment.
    /// Neither $Number$ nor the $Time$ identifier shall be included.
    /// </summary>
    public string? Initialization => helper.ParseOptionalString("initialization");

    /// <summary>
    /// Optional
    ///
    /// Specifies the template to create the Bitstream Switching Segment.
    /// Neither $Number$ nor the $Time$ identifier shall be included.
    /// </summary>
    public bool BitstreamSwitching => helper.ParseOptionalBool("bitstreamSwitching", false);

    public SegmentTimeline? SegmentTimeline => segmentTimeline;

    private SegmentTimeline? ParseSegmentTimeline()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "SegmentTimeline")
            .Select(n => new SegmentTimeline(n))
            .FirstOrDefault();
    }
}
