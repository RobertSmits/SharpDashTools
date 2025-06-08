using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdRepresentation : MpdElement
{
    private readonly Lazy<MpdSegmentList?> segmentList;
    private readonly Lazy<MpdSegmentTemplate?> segmentTemplate;
    private readonly Lazy<IEnumerable<MpdContentProtection>> contentProtections;
    private readonly Lazy<string?> baseURL;

    internal MpdRepresentation(XElement node)
        : base(node)
    {
        segmentList = new Lazy<MpdSegmentList?>(ParseSegmentList);
        segmentTemplate = new Lazy<MpdSegmentTemplate?>(ParseSegmentTemplate);
        contentProtections = new Lazy<IEnumerable<MpdContentProtection>>(ParseContentProtections);
        baseURL = new Lazy<string?>(ParseBaseURL);
    }

    public string? Id => helper.ParseOptionalString("id");

    public uint Bandwidth => helper.ParseMandatoryUint("bandwidth");

    public uint? QualityRanking => helper.ParseOptionalUint("qualityRanking");

    public string? DependencyId => helper.ParseOptionalString("dependencyId");

    public string? MediaStreamStructureId => helper.ParseOptionalString("mediaStreamStructureId");

    public MpdSegmentList? SegmentList => segmentList.Value;

    public MpdSegmentTemplate? SegmentTemplate => segmentTemplate.Value;

    public IEnumerable<MpdContentProtection> ContentProtections => contentProtections.Value;

    public string? BaseURL => baseURL.Value;

    private MpdSegmentList? ParseSegmentList()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "SegmentList")
            .Select(n => new MpdSegmentList(n))
            .FirstOrDefault();
    }

    private MpdSegmentTemplate? ParseSegmentTemplate()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "SegmentTemplate")
            .Select(n => new MpdSegmentTemplate(n))
            .FirstOrDefault();
    }
    private string? ParseBaseURL()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "BaseURL")
            .Select(n => n.Value)
            .FirstOrDefault();
    }

    private IEnumerable<MpdContentProtection> ParseContentProtections()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "ContentProtection")
            .Select(n => new MpdContentProtection(n));
    }
}
