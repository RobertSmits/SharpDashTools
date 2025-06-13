﻿using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class MpdInitialization : MpdElement
{
    internal MpdInitialization(XElement node)
        : base(node) { }

    public string SourceUrl => _node.ParseMandatoryString("sourceURL");
}
