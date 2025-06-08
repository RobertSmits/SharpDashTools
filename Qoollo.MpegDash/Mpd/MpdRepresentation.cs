using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdRepresentation : MpdElement
{
    private readonly Lazy<MpdSegmentList?> _segmentList;
    private readonly Lazy<MpdSegmentTemplate?> _segmentTemplate;
    private readonly Lazy<IEnumerable<MpdContentProtection>> _contentProtections;
    private readonly Lazy<string?> _baseURL;

    internal MpdRepresentation(XElement node)
        : base(node)
    {
        _segmentList = new Lazy<MpdSegmentList?>(ParseSegmentList);
        _segmentTemplate = new Lazy<MpdSegmentTemplate?>(ParseSegmentTemplate);
        _contentProtections = new Lazy<IEnumerable<MpdContentProtection>>(ParseContentProtections);
        _baseURL = new Lazy<string?>(ParseBaseURL);
    }

    public string? Id => _helper.ParseOptionalString("id");

    public uint Bandwidth => _helper.ParseMandatoryUint("bandwidth");

    public uint? QualityRanking => _helper.ParseOptionalUint("qualityRanking");

    public string? DependencyId => _helper.ParseOptionalString("dependencyId");

    public string? MediaStreamStructureId => _helper.ParseOptionalString("mediaStreamStructureId");

    public MpdSegmentList? SegmentList => _segmentList.Value;

    public MpdSegmentTemplate? SegmentTemplate => _segmentTemplate.Value;

    public IEnumerable<MpdContentProtection> ContentProtections => _contentProtections.Value;

    public string? BaseURL => _baseURL.Value;

    private MpdSegmentList? ParseSegmentList()
    {
        return _node
            .Elements()
            .Where(n => n.Name.LocalName == "SegmentList")
            .Select(n => new MpdSegmentList(n))
            .FirstOrDefault();
    }

    private MpdSegmentTemplate? ParseSegmentTemplate()
    {
        return _node
            .Elements()
            .Where(n => n.Name.LocalName == "SegmentTemplate")
            .Select(n => new MpdSegmentTemplate(n))
            .FirstOrDefault();
    }

    private string? ParseBaseURL()
    {
        return _node.Elements().Where(n => n.Name.LocalName == "BaseURL").Select(n => n.Value).FirstOrDefault();
    }

    private IEnumerable<MpdContentProtection> ParseContentProtections()
    {
        return _node
            .Elements()
            .Where(n => n.Name.LocalName == "ContentProtection")
            .Select(n => new MpdContentProtection(n));
    }
}
