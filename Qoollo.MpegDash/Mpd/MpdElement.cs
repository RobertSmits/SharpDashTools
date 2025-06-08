using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public abstract class MpdElement
{
    protected readonly XElement _node;
    protected readonly XmlAttributeParseHelper _helper;

    internal MpdElement(XElement node)
    {
        _node = node;
        _helper = new XmlAttributeParseHelper(node);
    }
}
