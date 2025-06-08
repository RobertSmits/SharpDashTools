using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdSegmentUrl : MpdElement
{
    internal MpdSegmentUrl(XElement node)
        : base(node) { }

    public int Index => _node.ParseMandatoryInt("index");

    public string Media => _node.ParseMandatoryString("media");
}
