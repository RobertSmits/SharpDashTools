﻿using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public abstract class MpdElement
{
    protected readonly XElement node;

    protected readonly XmlAttributeParseHelper helper;

    internal MpdElement(XElement node)
    {
        this.node = node;
        this.helper = new XmlAttributeParseHelper(node);
    }
}
