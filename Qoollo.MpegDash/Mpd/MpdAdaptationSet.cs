using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdAdaptationSet : MpdElement
{
    private readonly Lazy<MpdValue?> _audioChannelConfiguration;
    private readonly Lazy<MpdValue?> _accessibility;
    private readonly Lazy<MpdValue?> _role;
    private readonly Lazy<MpdSegmentTemplate?> _segmentTemplate;
    private readonly Lazy<IEnumerable<MpdRepresentation>> _representations;
    private readonly Lazy<IEnumerable<MpdContentProtection>> _contentProtections;

    internal MpdAdaptationSet(XElement node)
        : base(node)
    {
        _audioChannelConfiguration = new Lazy<MpdValue?>(ParseAudioChannelConfiguration);
        _accessibility = new Lazy<MpdValue?>(ParseAccessibility);
        _role = new Lazy<MpdValue?>(ParseRole);
        _segmentTemplate = new Lazy<MpdSegmentTemplate?>(ParseSegmentTemplate);
        _representations = new Lazy<IEnumerable<MpdRepresentation>>(ParseRepresentations);
        _contentProtections = new Lazy<IEnumerable<MpdContentProtection>>(ParseContentProtections);
    }

    public uint? Id => _helper.ParseOptionalUint("id");

    public uint? Group => _helper.ParseOptionalUint("group");

    public string? Lang => _node.Attribute("lang")?.Value;

    public string? ContentType => _helper.ParseOptionalString("contentType") ?? _helper.ParseOptionalString("mimeType");

    public AspectRatio? Par => _helper.ParseOptionalAspectRatio("par");

    public uint? MinBandwidth => _helper.ParseOptionalUint("minBandwidth");

    public uint? MaxBandwidth => _helper.ParseOptionalUint("maxBandwidth");

    public uint? MinWidth => _helper.ParseOptionalUint("minWidth");

    public uint? MaxWidth => _helper.ParseOptionalUint("maxWidth");

    public uint? MinHeight => _helper.ParseOptionalUint("minHeight");

    public uint? MaxHeight => _helper.ParseOptionalUint("maxHeight");

    public FrameRate? MinFrameRate => _helper.ParseOptionalFrameRate("minFrameRate");

    public FrameRate? MaxFrameRate => _helper.ParseOptionalFrameRate("maxFrameRate");

    public bool SegmentAlignment => _helper.ParseOptionalBool("segmentAlignment", false);

    public bool BitstreamSwitching => _helper.ParseOptionalBool("bitstreamSwitching", false);

    public bool SubsegmentAlignment => _helper.ParseOptionalBool("subsegmentAlignment", false);

    public uint? SubsegmentStartsWithSAP => _helper.ParseOptionalUint("subsegmentStartsWithSAP") ?? _helper.ParseOptionalUint("startWithSAP");

    public MpdValue? AudioChannelConfiguration => _audioChannelConfiguration.Value;

    public MpdValue? Accessibility => _accessibility.Value;

    public MpdValue? Role => _role.Value;

    /// <summary>
    /// Specifies default Segment Template information.
    ///
    /// Information in this element is overridden by information in
    /// AdapationSet.SegmentTemplate and
    /// Representation.SegmentTemplate, if present.
    /// </summary>
    public MpdSegmentTemplate? SegmentTemplate => _segmentTemplate.Value;

    public IEnumerable<MpdRepresentation> Representations => _representations.Value;

    public IEnumerable<MpdContentProtection> ContentProtections => _contentProtections.Value;

    private MpdValue? ParseAudioChannelConfiguration()
    {
        return _node.Elements()
            .Where(n => n.Name.LocalName == "AudioChannelConfiguration")
            .Select(n => new MpdValue(n))
            .FirstOrDefault();
    }

    private MpdValue? ParseAccessibility()
    {
        return _node.Elements()
            .Where(n => n.Name.LocalName == "Accessibility")
            .Select(n => new MpdValue(n))
            .FirstOrDefault();
    }

    private MpdValue? ParseRole()
    {
        return _node.Elements()
            .Where(n => n.Name.LocalName == "Role")
            .Select(n => new MpdValue(n))
            .FirstOrDefault();
    }

    private MpdSegmentTemplate? ParseSegmentTemplate()
    {
        return _node.Elements()
            .Where(n => n.Name.LocalName == "SegmentTemplate")
            .Select(n => new MpdSegmentTemplate(n))
            .FirstOrDefault();
    }

    private IEnumerable<MpdRepresentation> ParseRepresentations()
    {
        return _node.Elements()
            .Where(n => n.Name.LocalName == "Representation")
            .Select(n => new MpdRepresentation(n));
    }

    private IEnumerable<MpdContentProtection> ParseContentProtections()
    {
        return _node.Elements()
            .Where(n => n.Name.LocalName == "ContentProtection")
            .Select(n => new MpdContentProtection(n));
    }
}
