using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public class XmlAttributeParseHelper
{
    private readonly XElement node;

    public XmlAttributeParseHelper(XElement node)
    {
        this.node = node;
    }

    public string ParseMandatoryString(string attributeName)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? throw new Exception($"Attribute \"{attributeName}\" not found on element {node}")
            : attr.Value;
    }

    public string? ParseOptionalString(string attributeName)
    {
        var attr = node.Attribute(attributeName);
        return attr?.Value;
    }

    public DateTimeOffset? ParseDateTimeOffset(string attributeName, bool mandatoryCondition)
    {
        if (!mandatoryCondition && node.Attribute(attributeName) is null)
            throw new Exception($"MPD attribute @{attributeName} should be present.");
        return ParseOptionalDateTimeOffset(attributeName);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public DateTimeOffset? ParseOptionalDateTimeOffset(string attributeName, DateTimeOffset? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? defaultValue
            : DateTimeOffset.Parse(attr.Value);
    }

    public TimeSpan ParseMandatoryTimeSpan(string attributeName)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? throw new Exception($"MPD attribute @{attributeName} should be present.")
            : XmlConvert.ToTimeSpan(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TimeSpan? ParseOptionalTimeSpan(string attributeName, TimeSpan? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? defaultValue
            : XmlConvert.ToTimeSpan(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public bool ParseOptionalBool(string attributeName, bool defaultValue)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? defaultValue
            : bool.Parse(attr.Value);
    }

    public uint ParseMandatoryUint(string attributeName)
    {
        var attr = ParseMandatoryString(attributeName);
        return uint.Parse(attr);
    }

    public uint? ParseOptionalUint(string attributeName, uint? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? defaultValue
            : uint.Parse(attr.Value);
    }

    public int ParseMandatoryInt(string attributeName)
    {
        var attr = ParseMandatoryString(attributeName);
        return int.Parse(attr);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public int? ParseOptionalInt(string attributeName, int? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? defaultValue
            : int.Parse(attr.Value);
    }

    public ulong ParseMandatoryUlong(string attributeName)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? throw new Exception($"AtStribute \"{attributeName}\" not found on element {node}")
            : ulong.Parse(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public ulong? ParseOptionalUlong(string attributeName, ulong? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? defaultValue
            : ulong.Parse(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public double? ParseOptionalDouble(string attributeName, double? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? defaultValue
            : double.Parse(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public AspectRatio? ParseOptionalAspectRatio(string attributeName, AspectRatio? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? defaultValue
            : new AspectRatio(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public FrameRate? ParseOptionalFrameRate(string attributeName, FrameRate? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? defaultValue
            : new FrameRate(attr.Value);
    }
}
