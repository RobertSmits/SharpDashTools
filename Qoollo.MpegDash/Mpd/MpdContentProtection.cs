using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdContentProtection : MpdElement
{
    public MpdContentProtection(XElement node)
        : base(node)
    {
    }

    public string SchemeIdUri => helper.ParseMandatoryString("schemeIdUri");

    public string? Value => helper.ParseOptionalString("value");

    public string? DefaultKID => node.Attribute(XName.Get("default_KID", "urn:mpeg:cenc:2013"))?.Value;

    public string? Pssh => node.Element(XName.Get("pssh", "urn:mpeg:cenc:2013"))?.Value;

    public string? Pro => node.Element(XName.Get("pro", "urn:mpeg:cenc:2013"))?.Value;
}
