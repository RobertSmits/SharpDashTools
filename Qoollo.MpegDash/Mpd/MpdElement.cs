using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public abstract class MpdElement
{
    protected readonly XElement _node;

    internal MpdElement(XElement node)
    {
        _node = node;
    }
}
