using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdValue : MpdElement
{
    internal MpdValue(XElement node)
        : base(node)
    {
    }

    public string SchemeIdUri
    {
        get { return node.Attribute("schemeIdUri").Value; }
    }

    public string Value
    {
        get { return node.Attribute("value").Value; }
    }
}
