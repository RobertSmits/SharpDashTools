using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdPeriod : MpdElement
{
    private readonly Lazy<IEnumerable<BaseUrl>> _baseUrls;
    private readonly Lazy<SegmentBase?> _segmentBase;
    private readonly Lazy<MpdSegmentList?> _segmentList;
    private readonly Lazy<MpdSegmentTemplate?> _segmentTemplate;
    private readonly Lazy<AssetIdentifier?> _assetIdentifier;
    private readonly Lazy<IEnumerable<MpdAdaptationSet>> _adaptationSets;

    internal MpdPeriod(XElement node)
        : base(node)
    {
        _baseUrls = new Lazy<IEnumerable<BaseUrl>>(ParseBaseUrls);
        _segmentBase = new Lazy<SegmentBase?>(ParseSegmentBase);
        _segmentList = new Lazy<MpdSegmentList?>(ParseSegmentList);
        _segmentTemplate = new Lazy<MpdSegmentTemplate?>(ParseSegmentTemplate);
        _assetIdentifier = new Lazy<AssetIdentifier?>(ParseAssetIdentifier);
        _adaptationSets = new Lazy<IEnumerable<MpdAdaptationSet>>(ParseAdaptationSets);
    }

    public string? Id => _helper.ParseOptionalString("id");

    public TimeSpan? Start => _helper.ParseOptionalTimeSpan("start");

    public TimeSpan? Duration => _helper.ParseOptionalTimeSpan("duration");

    public bool BitstreamSwitching => _helper.ParseOptionalBool("bitstreamSwitching", false);

    /// <summary>
    /// 0...N
    ///
    /// Specifies a base URL that can be used for reference resolution
    /// and alternative URL selection
    /// </summary>
    public IEnumerable<BaseUrl> BaseUrls => _baseUrls.Value;

    private IEnumerable<BaseUrl> ParseBaseUrls()
    {
        return _node.Elements()
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
    public SegmentBase? SegmentBase => _segmentBase.Value;

    private SegmentBase? ParseSegmentBase()
    {
        return _node.Elements()
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
    public MpdSegmentList? SegmentList => _segmentList.Value;

    private MpdSegmentList? ParseSegmentList()
    {
        return _node.Elements()
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
    public MpdSegmentTemplate? SegmentTemplate => _segmentTemplate.Value;

    private MpdSegmentTemplate? ParseSegmentTemplate()
    {
        return _node.Elements()
            .Where(n => n.Name.LocalName == "SegmentTemplate")
            .Select(n => new MpdSegmentTemplate(n))
            .FirstOrDefault();
    }

    /// <summary>
    /// 0...1
    ///
    /// Specifies that this Period belongs to a certain asset.
    /// </summary>
    public AssetIdentifier? AssetIdentifier => _assetIdentifier.Value;

    private AssetIdentifier? ParseAssetIdentifier()
    {
        return _node.Elements()
            .Where(n => n.Name.LocalName == "AssetIdentifier")
            .Select(n => new AssetIdentifier(n))
            .FirstOrDefault();
    }

    public IEnumerable<MpdAdaptationSet> AdaptationSets => _adaptationSets.Value;

    private IEnumerable<MpdAdaptationSet> ParseAdaptationSets()
    {
        return _node.Elements()
            .Where(n => n.Name.LocalName == "AdaptationSet")
            .Select(n => new MpdAdaptationSet(n));
    }
}
