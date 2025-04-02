using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static NordicSpaceLink.LibUHD.NativeMethods;

namespace NordicSpaceLink.LibUHD;

internal ref struct MetaRangeBuf
{
    private uhd_meta_range_handle handle;

    internal ref uhd_meta_range_handle Handle => ref Unsafe.AsRef(ref handle);

    public MetaRangeBuf()
    {
        Raise(uhd_meta_range_make(out handle));
    }

    public readonly double Clip(double value, bool clip_step) { Raise(uhd_meta_range_clip(handle, value, clip_step, out var result), handle, uhd_meta_range_last_error); return result; }

    public readonly int Length
    {
        get
        {
            Raise(uhd_meta_range_size(handle, out var size), handle, uhd_meta_range_last_error);
            return (int)size;
        }
    }


    public readonly Range this[int index]
    {
        get
        {
            Raise(uhd_meta_range_at(handle, (nuint)index, out var rng), handle, uhd_meta_range_last_error);
            return rng;
        }
    }

    public void Add(Range value)
    {
        Raise(uhd_meta_range_push_back(handle, ref value), handle, uhd_meta_range_last_error);
    }

    public readonly string PPString
    {
        get
        {
            using var buf = new Pooled<char>(1024);
            Raise(uhd_meta_range_to_pp_string(handle, buf.Buffer, buf.Length), handle, uhd_meta_range_last_error);
            return new(buf.Buffer);
        }
    }

    public void Dispose()
    {
        Raise(uhd_meta_range_free(ref handle));
    }

    internal readonly MetaRange Build()
    {
        Range[] values = new Range[Length];
        for (int i = 0; i < values.Length; i++)
            values[i] = this[i];
        return new(values);
    }
}

public struct MetaRange
{
    private Range[] rng;
    private double start, stop, step;

    public MetaRange(IEnumerable<Range> ranges)
    {
        this.rng = ranges.ToArray();
        var allSteps = new List<double>();

        double? lastEnd = null;
        foreach (var range in rng)
        {
            allSteps.Add(range.Step);
            if (lastEnd is not null)
                allSteps.Add(range.Start - lastEnd ?? 0);
            lastEnd = range.Stop;
        }

        start = rng.Min(r => r.Start);
        stop = rng.Max(r => r.Stop);
        step = allSteps.Where(s => s != 0).Min();
    }

    public readonly IReadOnlyList<Range> Ranges => rng;

    public readonly double Start => start;
    public readonly double Stop => stop;
    public readonly double Step => step;

}