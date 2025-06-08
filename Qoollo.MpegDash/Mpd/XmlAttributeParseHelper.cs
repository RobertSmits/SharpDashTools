using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd;

public static class XmlAttributeParseExtensions
{
    public static string ParseMandatoryString(this XElement node, string attributeName)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? throw new Exception($"Attribute \"{attributeName}\" not found on element {node}")
            : attr.Value;
    }

    public static string? ParseOptionalString(this XElement node, string attributeName)
    {
        var attr = node.Attribute(attributeName);
        return attr?.Value;
    }

    public static DateTimeOffset? ParseDateTimeOffset(this XElement node, string attributeName, bool mandatoryCondition)
    {
        if (!mandatoryCondition && node.Attribute(attributeName) is null)
            throw new Exception($"MPD attribute @{attributeName} should be present.");
        return node.ParseOptionalDateTimeOffset(attributeName);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static DateTimeOffset? ParseOptionalDateTimeOffset(
        this XElement node,
        string attributeName,
        DateTimeOffset? defaultValue = null
    )
    {
        var attr = node.Attribute(attributeName);
        return attr is null ? defaultValue : DateTimeOffset.Parse(attr.Value);
    }

    public static TimeSpan ParseMandatoryTimeSpan(this XElement node, string attributeName)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? throw new Exception($"MPD attribute @{attributeName} should be present.")
            : XmlConvert.ToTimeSpan(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TimeSpan? ParseOptionalTimeSpan(
        this XElement node,
        string attributeName,
        TimeSpan? defaultValue = null
    )
    {
        var attr = node.Attribute(attributeName);
        return attr is null ? defaultValue : XmlConvert.ToTimeSpan(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static bool ParseOptionalBool(this XElement node, string attributeName, bool defaultValue)
    {
        var attr = node.Attribute(attributeName);
        return attr is null ? defaultValue : bool.Parse(attr.Value);
    }

    public static uint ParseMandatoryUint(this XElement node, string attributeName)
    {
        var attr = node.ParseMandatoryString(attributeName);
        return uint.Parse(attr);
    }

    public static uint? ParseOptionalUint(this XElement node, string attributeName, uint? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null ? defaultValue : uint.Parse(attr.Value);
    }

    public static int ParseMandatoryInt(this XElement node, string attributeName)
    {
        var attr = node.ParseMandatoryString(attributeName);
        return int.Parse(attr);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static int? ParseOptionalInt(this XElement node, string attributeName, int? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null ? defaultValue : int.Parse(attr.Value);
    }

    public static ulong ParseMandatoryUlong(this XElement node, string attributeName)
    {
        var attr = node.Attribute(attributeName);
        return attr is null
            ? throw new Exception($"Attribute \"{attributeName}\" not found on element {node}")
            : ulong.Parse(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static ulong? ParseOptionalUlong(this XElement node, string attributeName, ulong? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null ? defaultValue : ulong.Parse(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static double? ParseOptionalDouble(this XElement node, string attributeName, double? defaultValue = null)
    {
        var attr = node.Attribute(attributeName);
        return attr is null ? defaultValue : double.Parse(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static AspectRatio? ParseOptionalAspectRatio(
        this XElement node,
        string attributeName,
        AspectRatio? defaultValue = null
    )
    {
        var attr = node.Attribute(attributeName);
        return attr is null ? defaultValue : new AspectRatio(attr.Value);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static FrameRate? ParseOptionalFrameRate(
        this XElement node,
        string attributeName,
        FrameRate? defaultValue = null
    )
    {
        var attr = node.Attribute(attributeName);
        return attr is null ? defaultValue : new FrameRate(attr.Value);
    }
}
