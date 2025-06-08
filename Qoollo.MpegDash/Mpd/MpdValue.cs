using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdValue : MpdElement
{
    internal MpdValue(XElement node)
        : base(node) { }

    public string SchemeIdUri => _node.ParseMandatoryString("schemeIdUri");

    public string Value => _node.ParseMandatoryString("value");
}
