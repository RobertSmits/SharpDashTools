﻿using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdValue : MpdElement
{
    internal MpdValue(XElement node)
        : base(node)
    {
    }

    public string SchemeIdUri => helper.ParseMandatoryString("schemeIdUri");

    public string Value => helper.ParseMandatoryString("value");
}
