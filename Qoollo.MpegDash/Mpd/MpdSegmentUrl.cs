using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdSegmentUrl : MpdElement
{
    internal MpdSegmentUrl(XElement node)
        : base(node)
    {
    }

    public int Index => helper.ParseMandatoryInt("index");

    public string Media => helper.ParseMandatoryString("media");
}
