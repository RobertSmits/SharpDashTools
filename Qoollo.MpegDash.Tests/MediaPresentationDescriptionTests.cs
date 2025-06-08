using Qoollo.MpegDash.Mpd;
using Xunit;

namespace Qoollo.MpegDash.Tests;

public class MediaPresentationDescriptionTests : IClassFixture<MpdFixture>
{
    private MediaPresentationDescription _mpd;

    public MediaPresentationDescriptionTests(MpdFixture mpdFixture)
    {
        _mpd = mpdFixture.Mpd;
    }

    [Fact]
    public void Id()
    {
        Assert.Null(_mpd.Id);
    }

    [Fact]
    public void Profiles()
    {
        Assert.Equal("urn:mpeg:dash:profile:isoff-live:2011", _mpd.Profiles);
    }

    [Fact]
    public void Type()
    {
        Assert.Equal("static", _mpd.Type);
    }

    [Fact]
    public void AvailabilityStartTime()
    {
        Assert.Equal(new DateTimeOffset(2016, 1, 20, 21, 10, 2, TimeSpan.Zero), _mpd.AvailabilityStartTime);
    }

    [Fact]
    public void PublishTime()
    {
        Assert.Null(_mpd.PublishTime);
    }

    [Fact]
    public void AvailabilityEndTime()
    {
        Assert.Null(_mpd.AvailabilityEndTime);
    }

    [Fact]
    public void MediaPresentationDuration()
    {
        Assert.Equal(TimeSpan.FromSeconds(193.680), _mpd.MediaPresentationDuration);
    }

    [Fact]
    public void MinimumUpdatePeriod()
    {
        Assert.Null(_mpd.MinimumUpdatePeriod);
    }

    [Fact]
    public void MinBufferTime()
    {
        Assert.Equal(TimeSpan.FromSeconds(5), _mpd.MinBufferTime);
    }

    [Fact]
    public void TimeShiftBufferDepth()
    {
        Assert.Null(_mpd.TimeShiftBufferDepth);
    }

    [Fact]
    public void SuggestedPresentationDelay()
    {
        Assert.Null(_mpd.SuggestedPresentationDelay);
    }

    [Fact]
    public void MaxSegmentDuration()
    {
        Assert.Equal(TimeSpan.FromMilliseconds(2005), _mpd.MaxSegmentDuration);
    }

    [Fact]
    public void MaxSubsegmentDuration()
    {
        Assert.Null(_mpd.MaxSubsegmentDuration);
    }

    [Fact]
    public void Periods_Count()
    {
        Assert.Equal(1, _mpd.Periods.Count());
    }

    [Fact]
    public void Period_Id()
    {
        Assert.Equal("period0", _mpd.Periods.First().Id);
    }

    [Fact]
    public void Period_Start()
    {
        Assert.Null(_mpd.Periods.First().Start);
    }

    [Fact]
    public void Period_Duration()
    {
        Assert.Null(_mpd.Periods.First().Duration);
    }

    [Fact]
    public void Period_BitstreamSwitching()
    {
        Assert.False(_mpd.Periods.First().BitstreamSwitching);
    }

    [Fact]
    public void Period_AdaptationSets_Count()
    {
        Assert.Equal(2, _mpd.Periods.First().AdaptationSets.Count());
    }

    [Fact]
    public void Period_AdaptationSets_0_Id()
    {
        Assert.Null(_mpd.Periods.First().AdaptationSets.First().Id);
    }

    [Fact]
    public void Period_AdaptationSets_0_Group()
    {
        Assert.Null(_mpd.Periods.First().AdaptationSets.First().Group);
    }

    [Fact]
    public void Period_AdaptationSets_0_Lang()
    {
        Assert.Null(_mpd.Periods.First().AdaptationSets.First().Lang);
    }

    [Fact]
    public void Period_AdaptationSets_0_ContentType()
    {
        Assert.Equal("video/mp4", _mpd.Periods.First().AdaptationSets.First().ContentType);
    }

    [Fact]
    public void Period_AdaptationSets_0_Par()
    {
        Assert.Equal("1:1", _mpd.Periods.First().AdaptationSets.First().Par?.RawValue);
    }

    [Fact]
    public void Period_AdaptationSets_0_MinBandwidth()
    {
        Assert.Null(_mpd.Periods.First().AdaptationSets.First().MinBandwidth);
    }

    [Fact]
    public void Period_AdaptationSets_0_MaxBandwidth()
    {
        Assert.Null(_mpd.Periods.First().AdaptationSets.First().MaxBandwidth);
    }

    [Fact]
    public void Period_AdaptationSets_0_MinWidth()
    {
        Assert.Null(_mpd.Periods.First().AdaptationSets.First().MinWidth);
    }

    [Fact]
    public void Period_AdaptationSets_0_MaxWidth()
    {
        Assert.Equal(1920u, _mpd.Periods.First().AdaptationSets.First().MaxWidth);
    }

    [Fact]
    public void Period_AdaptationSets_0_MinHeight()
    {
        Assert.Null(_mpd.Periods.First().AdaptationSets.First().MinHeight);
    }

    [Fact]
    public void Period_AdaptationSets_0_MaxHeight()
    {
        Assert.Equal(1080u, _mpd.Periods.First().AdaptationSets.First().MaxHeight);
    }

    [Fact]
    public void Period_AdaptationSets_0_MinFrameRate()
    {
        Assert.Null(_mpd.Periods.First().AdaptationSets.First().MinFrameRate);
    }

    [Fact]
    public void Period_AdaptationSets_0_MaxFrameRate()
    {
        Assert.Equal("30000/1001", _mpd.Periods.First().AdaptationSets.First().MaxFrameRate?.RawValue);
    }

    [Fact]
    public void Period_AdaptationSets_0_SegmentAlignment()
    {
        Assert.True(_mpd.Periods.First().AdaptationSets.First().SegmentAlignment);
    }

    [Fact]
    public void Period_AdaptationSets_0_BitstreamSwitching()
    {
        Assert.False(_mpd.Periods.First().AdaptationSets.First().BitstreamSwitching);
    }

    [Fact]
    public void Period_AdaptationSets_0_SubsegmentAlignment()
    {
        Assert.False(_mpd.Periods.First().AdaptationSets.First().SubsegmentAlignment);
    }

    [Fact]
    public void Period_AdaptationSets_0_SubsegmentStartsWithSAP()
    {
        Assert.Equal(1u, _mpd.Periods.First().AdaptationSets.First().SubsegmentStartsWithSAP);
    }
}
