using Qoollo.MpegDash.Mpd;

namespace Qoollo.MpegDash;

public class Track
{
    private readonly MpdAdaptationSet _adaptationSet;
    private readonly Lazy<IEnumerable<TrackRepresentation>> _trackRepresentations;

    public Track(MpdAdaptationSet adaptationSet)
    {
        _adaptationSet = adaptationSet;
        _trackRepresentations = new Lazy<IEnumerable<TrackRepresentation>>(GetTrackRepresentations);
    }

    public string? ContentType => _adaptationSet.ContentType;

    public IEnumerable<TrackRepresentation> TrackRepresentations => _trackRepresentations.Value;

    private IEnumerable<TrackRepresentation> GetTrackRepresentations()
    {
        return _adaptationSet.Representations.Select(r => new TrackRepresentation(_adaptationSet, r));
    }
}
