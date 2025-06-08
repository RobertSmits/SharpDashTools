using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdPeriod : MpdElement
{
    private readonly Lazy<IEnumerable<BaseUrl>> baseUrls;
    private readonly Lazy<SegmentBase?> segmentBase;
    private readonly Lazy<MpdSegmentList?> segmentList;
    private readonly Lazy<MpdSegmentTemplate?> segmentTemplate;
    private readonly Lazy<AssetIdentifier?> assetIdentifier;
    private readonly Lazy<IEnumerable<MpdAdaptationSet>> adaptationSets;

    internal MpdPeriod(XElement node)
        : base(node)
    {
        baseUrls = new Lazy<IEnumerable<BaseUrl>>(ParseBaseUrls);
        segmentBase = new Lazy<SegmentBase?>(ParseSegmentBase);
        segmentList = new Lazy<MpdSegmentList?>(ParseSegmentList);
        segmentTemplate = new Lazy<MpdSegmentTemplate?>(ParseSegmentTemplate);
        assetIdentifier = new Lazy<AssetIdentifier?>(ParseAssetIdentifier);
        adaptationSets = new Lazy<IEnumerable<MpdAdaptationSet>>(ParseAdaptationSets);
    }

    public string? Id => helper.ParseOptionalString("id");

    public TimeSpan? Start => helper.ParseOptionalTimeSpan("start");

    public TimeSpan? Duration => helper.ParseOptionalTimeSpan("duration");

    public bool BitstreamSwitching => helper.ParseOptionalBool("bitstreamSwitching", false);

    /// <summary>
    /// 0...N
    ///
    /// Specifies a base URL that can be used for reference resolution
    /// and alternative URL selection
    /// </summary>
    public IEnumerable<BaseUrl> BaseUrls => baseUrls.Value;

    private IEnumerable<BaseUrl> ParseBaseUrls()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "BaseURL")
            .Select(n => new BaseUrl(n));
    }

    /// <summary>
    /// 0...1
    ///
    /// Specifies default Segment Base information.
    ///
    /// Information in this element is overridden by information in
    /// AdapationSet.SegmentBase and Representation.SegmentBase, if present.
    /// </summary>
    public SegmentBase? SegmentBase => segmentBase.Value;

    private SegmentBase? ParseSegmentBase()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "SegmentBase")
            .Select(n => new SegmentBase(n))
            .FirstOrDefault();
    }

    /// <summary>
    /// 0...1
    ///
    /// Specifies default Segment List information.
    ///
    /// Information in this element is overridden by information in
    /// AdapationSet.SegmentList and Representation.SegmentList, if present.
    /// </summary>
    public MpdSegmentList? SegmentList => segmentList.Value;

    private MpdSegmentList? ParseSegmentList()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "SegmentList")
            .Select(n => new MpdSegmentList(n))
            .FirstOrDefault();
    }

    /// <summary>
    /// 0...1
    ///
    /// Specifies default Segment Template information.
    ///
    /// Information in this element is overridden by information in
    /// AdapationSet.SegmentTemplate and Representation.SegmentTemplate, if present.
    /// </summary>
    public MpdSegmentTemplate? SegmentTemplate => segmentTemplate.Value;

    private MpdSegmentTemplate? ParseSegmentTemplate()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "SegmentTemplate")
            .Select(n => new MpdSegmentTemplate(n))
            .FirstOrDefault();
    }

    /// <summary>
    /// 0...1
    ///
    /// Specifies that this Period belongs to a certain asset.
    /// </summary>
    public AssetIdentifier? AssetIdentifier => assetIdentifier.Value;

    private AssetIdentifier? ParseAssetIdentifier()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "AssetIdentifier")
            .Select(n => new AssetIdentifier(n))
            .FirstOrDefault();
    }

    public IEnumerable<MpdAdaptationSet> AdaptationSets => adaptationSets.Value;

    private IEnumerable<MpdAdaptationSet> ParseAdaptationSets()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "AdaptationSet")
            .Select(n => new MpdAdaptationSet(n));
    }
}
