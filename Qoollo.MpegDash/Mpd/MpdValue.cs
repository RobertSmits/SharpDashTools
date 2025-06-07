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
        get { return helper.ParseMandatoryString("schemeIdUri"); }
    }

    public string Value
    {
        get { return helper.ParseMandatoryString("value"); }
    }
}
