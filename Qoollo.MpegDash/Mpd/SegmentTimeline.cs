using System.Collections;
using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class SegmentTimeline : MpdElement, IEnumerable<SegmentTimelineItem>
{
    private readonly IEnumerable<SegmentTimelineItem> items;

    public SegmentTimeline(XElement node)
        : base(node)
    {
        items = ParseItems();
    }

    private IEnumerable<SegmentTimelineItem> ParseItems()
    {
        return node.Elements()
            .Where(n => n.Name.LocalName == "S")
            .Select(n => new SegmentTimelineItem(n));
    }

    public IEnumerator<SegmentTimelineItem> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }
}
