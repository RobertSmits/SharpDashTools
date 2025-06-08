using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class BaseUrl : MpdElement
{
    public BaseUrl(XElement node)
        : base(node) { }

    public string Value => _node.Value;
}
