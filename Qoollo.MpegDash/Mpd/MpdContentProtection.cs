using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdContentProtection : MpdElement
{
    public MpdContentProtection(XElement node)
        : base(node)
    {
    }

    public string SchemeIdUri
    {
        get { return helper.ParseMandatoryString("schemeIdUri"); }
    }
    public string? Value
    {
        get { return helper.ParseOptionalString("value"); }
    }
    public string? DefaultKID
    {
        get { return node.Attribute(XName.Get("default_KID", "urn:mpeg:cenc:2013"))?.Value; }
    }
    public string? Pssh
    {
        get { return node.Element(XName.Get("pssh", "urn:mpeg:cenc:2013"))?.Value; }
    }
    public string? Pro
    {
        get { return node.Element(XName.Get("pro", "urn:mpeg:cenc:2013"))?.Value; }
    }
}
