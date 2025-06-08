using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdContentProtection : MpdElement
{
    public MpdContentProtection(XElement node)
        : base(node)
    {
    }

    public string SchemeIdUri => _helper.ParseMandatoryString("schemeIdUri");

    public string? Value => _helper.ParseOptionalString("value");

    public string? DefaultKID => _node.Attribute(XName.Get("default_KID", "urn:mpeg:cenc:2013"))?.Value;

    public string? Pssh => _node.Element(XName.Get("pssh", "urn:mpeg:cenc:2013"))?.Value;

    public string? Pro => _node.Element(XName.Get("pro", "urn:mpeg:cenc:2013"))?.Value;
}
