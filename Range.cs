using System.Runtime.InteropServices;
using static NordicSpaceLink.LibUHD.NativeMethods;

namespace NordicSpaceLink.LibUHD;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Range
{
    public readonly double Start;
    public readonly double Stop;
    public readonly double Step;

    public readonly string PPString
    {
        get
        {
            using var buf = new Pooled<char>(1024);
            var copy = this;
            Raise(uhd_range_to_pp_string(ref copy, buf.Buffer, buf.Length));
            return new(buf.Buffer);
        }
    }
}