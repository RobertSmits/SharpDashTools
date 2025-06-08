using Qoollo.MpegDash.Mpd;

namespace Qoollo.MpegDash;

public class TrackRepresentation
{
    private readonly MpdAdaptationSet _adaptationSet;
    private readonly MpdRepresentation _representation;
    private readonly Lazy<string> _initFragmentPath;
    private readonly Lazy<IEnumerable<string>> _fragmentsPaths;

    public TrackRepresentation(MpdAdaptationSet adaptationSet, MpdRepresentation representation)
    {
        _adaptationSet = adaptationSet;
        _representation = representation;

        _initFragmentPath = new Lazy<string>(GetInitFragmentPath);
        _fragmentsPaths = new Lazy<IEnumerable<string>>(GetFragmentsPaths);
    }

    public string InitFragmentPath => _initFragmentPath.Value;

    public IEnumerable<string> FragmentsPaths => _fragmentsPaths.Value;

    public uint Bandwidth => _representation.Bandwidth;

    private string GetInitFragmentPath()
    {
        var segmentTemplate = _representation.SegmentTemplate ?? _adaptationSet.SegmentTemplate;
        return segmentTemplate switch
        {
            // If SegmentTemplate has Initialization and Representation ID is available
            _ when segmentTemplate?.Initialization is not null && _representation.Id is not null =>
                segmentTemplate.Initialization.Replace("$RepresentationID$", _representation.Id),

            // If SegmentList has Initialization and SourceUrl is available
            _ when _representation.SegmentList?.Initialization?.SourceUrl is not null =>
                _representation.SegmentList.Initialization.SourceUrl,

            // If BaseURL is available
            _ when _representation.BaseURL is not null =>
                _representation.BaseURL,

            // If nothing matches, throw an exception
            _ => throw new Exception("Failed to determine InitFragmentPath")
        };
    }

    private IEnumerable<string> GetFragmentsPaths()
    {
        var segmentTemplate = _representation.SegmentTemplate ?? _adaptationSet.SegmentTemplate;
        if (segmentTemplate?.Media is not null && _representation.Id is not null)
        {
            if (segmentTemplate.SegmentTimeline != null)
            {
                ulong currentTime = 0;
                ulong currentSegment = segmentTemplate.StartNumber ?? 0;

                foreach (var segment in segmentTemplate.SegmentTimeline)
                {
                    currentTime = segment.Time ?? currentTime;

                    for (ulong i = 0; i <= (uint)segment.RepeatCount; i++)
                    {
                        var segmentUrl = segmentTemplate.Media
                            .Replace("$RepresentationID$", _representation.Id)
                            .Replace("$Time$", currentTime.ToString())
                            .Replace("$Number$", currentSegment.ToString());

                        currentTime += segment.Duration;
                        currentSegment++;
                        yield return segmentUrl;
                    }
                }
            }
            else
            {
                int i = 1;
                while (true) // ToDo break while when done. But when is done?
                {
                    yield return segmentTemplate.Media
                        .Replace("$RepresentationID$", _representation.Id)
                        .Replace("$Number$", i.ToString());
                    i++;
                }
            }
        }
        else if (_representation.SegmentList != null)
        {
            foreach (var segmentUrl in _representation.SegmentList.SegmentUrls.OrderBy(s => s.Index))
            {
                yield return segmentUrl.Media;
            }
        }
        else
            throw new Exception("Failed to determine FragmentPath");
    }

    private IEnumerable<TrackRepresentationSegment> GetSegments()
    {
        var segmentTemplate = _adaptationSet.SegmentTemplate ?? _representation.SegmentTemplate;
        if (segmentTemplate?.Media is not null && _representation.Id is not null)
        {
            var segments = GetSegmentsFromTimeline(segmentTemplate);

            bool hasTimelineItems = false;
            foreach (var segment in segments)
            {
                hasTimelineItems = true;

                yield return segment;
            }

            if (!hasTimelineItems)
            {
                segments = GetSegmentsFromRepresentation(_representation);
                foreach (var segment in segments)
                {
                    yield return segment;
                }
            }

        }
        else if (_representation.SegmentList is not null && _representation.SegmentList.Duration.HasValue)
        {
            foreach (var segmentUrl in _representation.SegmentList.SegmentUrls.OrderBy(s => s.Index))
            {
                yield return new TrackRepresentationSegment
                {
                    Path = segmentUrl.Media,
                    Duration = TimeSpan.FromMilliseconds(_representation.SegmentList.Duration.Value)
                };
            }
        }
        else
            // no segments (it's possible)
            yield break;
    }

    private IEnumerable<TrackRepresentationSegment> GetSegmentsFromRepresentation(MpdRepresentation representation)
    {
        if (representation.SegmentTemplate?.Media is null || !representation.SegmentTemplate.Duration.HasValue)
            yield break;

        int i = 1;
        while (true)
        {
            yield return new TrackRepresentationSegment
            {
                Path = representation.SegmentTemplate.Media
                    .Replace("$RepresentationID$", representation.Id)
                    .Replace("$Number$", i.ToString()),
                Duration = TimeSpan.FromMilliseconds(representation.SegmentTemplate.Duration.Value)
            };
            i++;
        }
    }

    private IEnumerable<TrackRepresentationSegment> GetSegmentsFromTimeline(MpdSegmentTemplate segmentTemplate)
    {
        if (segmentTemplate.Media is null || segmentTemplate.SegmentTimeline is null || _representation.Id is null)
            yield break;

        int i = 1;
        foreach (var item in segmentTemplate.SegmentTimeline)
        {
            int count = Math.Max(1, item.RepeatCount);
            for (int j = 0; j < count; j++)
            {
                yield return new TrackRepresentationSegment
                {
                    Path = segmentTemplate.Media
                        .Replace("$RepresentationID$", _representation.Id)
                        .Replace("$Number$", i.ToString()),
                    Duration = TimeSpan.FromMilliseconds(item.Duration)
                };
                i++;
            }
        }
    }

    internal IEnumerable<string> GetFragmentsPaths(TimeSpan from, TimeSpan to)
    {
        var span = TimeSpan.Zero;
        foreach (var segment in GetSegments())
        {
            if (span >= from && span + segment.Duration <= to)
                yield return segment.Path;

            span += segment.Duration;

            if (span > to)
                break;
        }
    }
}
