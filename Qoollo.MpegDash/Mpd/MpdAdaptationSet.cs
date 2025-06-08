using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdAdaptationSet : MpdElement
{
    private readonly Lazy<MpdValue?> audioChannelConfiguration;
    private readonly Lazy<MpdValue?> accessibility;
    private readonly Lazy<MpdValue?> role;
    private readonly Lazy<MpdSegmentTemplate?> segmentTemplate;
    private readonly Lazy<IEnumerable<MpdRepresentation>> representations;
    private readonly Lazy<IEnumerable<MpdContentProtection>> contentProtections;

    internal MpdAdaptationSet(XElement node)
        : base(node)
    {
        audioChannelConfiguration = new Lazy<MpdValue?>(ParseAudioChannelConfiguration);
        accessibility = new Lazy<MpdValue?>(ParseAccessibility);
        role = new Lazy<MpdValue?>(ParseRole);
        segmentTemplate = new Lazy<MpdSegmentTemplate?>(ParseSegmentTemplate);
        representations = new Lazy<IEnumerable<MpdRepresentation>>(ParseRepresentations);
        contentProtections = new Lazy<IEnumerable<MpdContentProtection>>(ParseContentProtections);
    }

    public uint? Id => helper.ParseOptionalUint("id");

    public uint? Group => helper.ParseOptionalUint("group");

    public string? Lang => node.Attribute("lang")?.Value;

    public string? ContentType => helper.ParseOptionalString("contentType") ?? helper.ParseOptionalString("mimeType");

    public AspectRatio? Par => helper.ParseOptionalAspectRatio("par");

    public uint? MinBandwidth => helper.ParseOptionalUint("minBandwidth");

    public uint? MaxBandwidth => helper.ParseOptionalUint("maxBandwidth");

    public uint? MinWidth => helper.ParseOptionalUint("minWidth");

    public uint? MaxWidth => helper.ParseOptionalUint("maxWidth");

    public uint? MinHeight => helper.ParseOptionalUint("minHeight");

    public uint? MaxHeight => helper.ParseOptionalUint("maxHeight");

    public FrameRate? MinFrameRate => helper.ParseOptionalFrameRate("minFrameRate");

    public FrameRate? MaxFrameRate => helper.ParseOptionalFrameRate("maxFrameRate");

    public bool SegmentAlignment => helper.ParseOptionalBool("segmentAlignment", false);

    public bool BitstreamSwitching => helper.ParseOptionalBool("bitstreamSwitching", false);

    public bool SubsegmentAlignment => helper.ParseOptionalBool("subsegmentAlignment", false);

    public uint? SubsegmentStartsWithSAP => helper.ParseOptionalUint("subsegmentStartsWithSAP") ?? helper.ParseOptionalUint("startWithSAP");

    public MpdValue? AudioChannelConfiguration => audioChannelConfiguration.Value;

    public MpdValue? Accessibility => accessibility.Value;

    public MpdValue? Role => role.Value;

    /// <summary>
    /// Specifies default Segment Template information.
    ///
    /// Information in this element is overridden by information in
    /// AdapationSet.SegmentTemplate and
    /// Representation.SegmentTemplate, if present.
    /// </summary>
    public MpdSegmentTemplate? SegmentTemplate => segmentTemplate.Value;

    public IEnumerable<MpdRepresentation> Representations => representations.Value;

    public IEnumerable<MpdContentProtection> ContentProtections => contentProtections.Value;

    private MpdValue? ParseAudioChannelConfiguration()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "AudioChannelConfiguration")
            .Select(n => new MpdValue(n))
            .FirstOrDefault();
    }

    private MpdValue? ParseAccessibility()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "Accessibility")
            .Select(n => new MpdValue(n))
            .FirstOrDefault();
    }

    private MpdValue? ParseRole()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "Role")
            .Select(n => new MpdValue(n))
            .FirstOrDefault();
    }

    private MpdSegmentTemplate? ParseSegmentTemplate()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "SegmentTemplate")
            .Select(n => new MpdSegmentTemplate(n))
            .FirstOrDefault();
    }

    private IEnumerable<MpdRepresentation> ParseRepresentations()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "Representation")
            .Select(n => new MpdRepresentation(n));
    }

    private IEnumerable<MpdContentProtection> ParseContentProtections()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "ContentProtection")
            .Select(n => new MpdContentProtection(n));
    }
}
