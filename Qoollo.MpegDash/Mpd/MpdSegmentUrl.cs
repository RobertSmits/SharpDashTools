using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdSegmentUrl : MpdElement
{
    internal MpdSegmentUrl(XElement node)
        : base(node)
    {
    }

    public int Index
    {
        get { return int.Parse(node.Attribute("index").Value); }
    }

    public string Media
    {
        get { return node.Attribute("media")?.Value; }
    }
}
