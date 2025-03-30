using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdAdaptationSet : MpdElement
{
    internal MpdAdaptationSet(XElement node)
        : base(node)
    {
        audioChannelConfiguration = new Lazy<MpdValue>(ParseAudioChannelConfiguration);
        accessibility = new Lazy<MpdValue>(ParseAccessibility);
        role = new Lazy<MpdValue>(ParseRole);
        segmentTemplate = new Lazy<MpdSegmentTemplate>(ParseSegmentTemplate);
        representations = new Lazy<IEnumerable<MpdRepresentation>>(ParseRepresentations);
        contentProtections = new Lazy<IEnumerable<MpdContentProtection>>(ParseContentProtections);
    }

    public uint? Id
    {
        get { return helper.ParseOptionalUint("id"); }
    }

    public uint? Group
    {
        get { return helper.ParseOptionalUint("group"); }
    }

    public string Lang
    {
        get { return node.Attribute("lang")?.Value; }
    }

    public string ContentType
    {
        get
        {
            var attr = node.Attribute("contentType") ?? node.Attribute("mimeType");
            return attr?.Value;
        }
    }

    public AspectRatio Par
    {
        get { return helper.ParseOptionalAspectRatio("par"); }
    }

    public uint? MinBandwidth
    {
        get { return helper.ParseOptionalUint("minBandwidth"); }
    }

    public uint? MaxBandwidth
    {
        get { return helper.ParseOptionalUint("maxBandwidth"); }
    }

    public uint? MinWidth
    {
        get { return helper.ParseOptionalUint("minWidth"); }
    }

    public uint? MaxWidth
    {
        get { return helper.ParseOptionalUint("maxWidth"); }
    }

    public uint? MinHeight
    {
        get { return helper.ParseOptionalUint("minHeight"); }
    }

    public uint? MaxHeight
    {
        get { return helper.ParseOptionalUint("maxHeight"); }
    }

    public FrameRate MinFrameRate
    {
        get { return helper.ParseOptionalFrameRate("minFrameRate"); }
    }

    public FrameRate MaxFrameRate
    {
        get { return helper.ParseOptionalFrameRate("maxFrameRate"); }
    }

    public bool SegmentAlignment
    {
        get { return helper.ParseOptionalBool("segmentAlignment", false); }
    }

    public bool BitstreamSwitching
    {
        get { return helper.ParseOptionalBool("bitstreamSwitching", false); }
    }

    public bool SubsegmentAlignment
    {
        get { return helper.ParseOptionalBool("subsegmentAlignment", false); }
    }

    public uint SubsegmentStartsWithSAP
    {
        get
        {
            var value = helper.ParseOptionalUint("subsegmentStartsWithSAP", null)
                ?? helper.ParseOptionalUint("startWithSAP", null);
            return value.Value;
        }
    }

    public MpdValue? AudioChannelConfiguration
    {
        get { return audioChannelConfiguration.Value; }
    }
    private readonly Lazy<MpdValue?> audioChannelConfiguration;

    public MpdValue? Accessibility
    {
        get { return accessibility.Value; }
    }
    private readonly Lazy<MpdValue?> accessibility;

    public MpdValue? Role
    {
        get { return role.Value; }
    }
    private readonly Lazy<MpdValue?> role;

    /// <summary>
    /// Specifies default Segment Template information.
    ///
    /// Information in this element is overridden by information in
    /// AdapationSet.SegmentTemplate and
    /// Representation.SegmentTemplate, if present.
    /// </summary>
    public MpdSegmentTemplate SegmentTemplate
    {
        get { return segmentTemplate.Value; }
    }
    private readonly Lazy<MpdSegmentTemplate> segmentTemplate;

    public IEnumerable<MpdRepresentation> Representations
    {
        get { return representations.Value; }
    }
    private readonly Lazy<IEnumerable<MpdRepresentation>> representations;

    public IEnumerable<MpdContentProtection> ContentProtections
    {
        get { return contentProtections.Value; }
    }
    private readonly Lazy<IEnumerable<MpdContentProtection>> contentProtections;

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

    private MpdSegmentTemplate ParseSegmentTemplate()
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
