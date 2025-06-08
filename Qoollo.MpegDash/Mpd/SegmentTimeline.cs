using System.Collections;
using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class SegmentTimeline : MpdElement, IEnumerable<SegmentTimelineItem>
{
    private readonly IEnumerable<SegmentTimelineItem> _items;

    public SegmentTimeline(XElement node)
        : base(node)
    {
        _items = ParseItems();
    }

    private IEnumerable<SegmentTimelineItem> ParseItems()
    {
        return _node.Elements().Where(n => n.Name.LocalName == "S").Select(n => new SegmentTimelineItem(n));
    }

    public IEnumerator<SegmentTimelineItem> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }
}
