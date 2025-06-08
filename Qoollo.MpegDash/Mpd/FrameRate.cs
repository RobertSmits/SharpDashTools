﻿namespace Qoollo.MpegDash.Mpd;

public class FrameRate
{
    public FrameRate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));

        RawValue = value;
    }

    public string RawValue { get; }
}
