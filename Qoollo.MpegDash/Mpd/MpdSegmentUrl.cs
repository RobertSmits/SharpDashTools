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
        get { return helper.ParseMandatoryInt("index"); }
    }

    public string Media
    {
        get { return helper.ParseMandatoryString("media"); }
    }
}
