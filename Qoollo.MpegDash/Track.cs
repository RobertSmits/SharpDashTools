using Qoollo.MpegDash.Mpd;

namespace Qoollo.MpegDash;

public class Track
{
    private readonly MpdAdaptationSet adaptationSet;
    private readonly Lazy<IEnumerable<TrackRepresentation>> trackRepresentations;

    public Track(MpdAdaptationSet adaptationSet)
    {
        this.adaptationSet = adaptationSet;
        trackRepresentations = new Lazy<IEnumerable<TrackRepresentation>>(GetTrackRepresentations);
    }

    public string? ContentType => adaptationSet.ContentType;

    public IEnumerable<TrackRepresentation> TrackRepresentations => trackRepresentations.Value;

    private IEnumerable<TrackRepresentation> GetTrackRepresentations()
    {
        return adaptationSet.Representations.Select(r => new TrackRepresentation(adaptationSet, r));
    }
}
