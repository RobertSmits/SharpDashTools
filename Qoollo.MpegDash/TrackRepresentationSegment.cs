namespace Qoollo.MpegDash;

public class TrackRepresentationSegment
{
    public TimeSpan Duration { get; set; }
    public int Repeat { get; set; }
    public string Path { get; set; } = string.Empty;
}
