using System.Runtime.CompilerServices;
using static NordicSpaceLink.LibUHD.NativeMethods;

namespace NordicSpaceLink.LibUHD;

public ref struct RxMetadata
{
    private uhd_rx_metadata_handle handle;

    internal ref uhd_rx_metadata_handle Handle => ref Unsafe.AsRef(ref handle);

    public readonly bool HasTimeSpec { get { Raise(uhd_rx_metadata_has_time_spec(handle, out var result), handle, uhd_rx_metadata_last_error); return result; } }
    public readonly bool MoreFragments { get { Raise(uhd_rx_metadata_more_fragments(handle, out var result), handle, uhd_rx_metadata_last_error); return result; } }
    public readonly nuint FragmentOffset { get { Raise(uhd_rx_metadata_fragment_offset(handle, out var result), handle, uhd_rx_metadata_last_error); return result; } }
    public readonly bool StartOfBurst { get { Raise(uhd_rx_metadata_start_of_burst(handle, out var result), handle, uhd_rx_metadata_last_error); return result; } }
    public readonly bool EndOfBurst { get { Raise(uhd_rx_metadata_end_of_burst(handle, out var result), handle, uhd_rx_metadata_last_error); return result; } }
    public readonly bool OutOfSequence { get { Raise(uhd_rx_metadata_out_of_sequence(handle, out var result), handle, uhd_rx_metadata_last_error); return result; } }

    public readonly (long full_secs_out, double frac_secs_out) TimeSpec { get { Raise(uhd_rx_metadata_time_spec(handle, out var a, out var b), handle, uhd_rx_metadata_last_error); return (a, b); } }

    public readonly string PPString
    {
        get
        {
            using var buf = new Pooled<char>(1024);
            Raise(uhd_rx_metadata_to_pp_string(handle, buf.Buffer, buf.Length), handle, uhd_rx_metadata_last_error);
            return new(buf.Buffer);
        }
    }

    public readonly RxMetadataErrorCode ErrorCode { get { Raise(uhd_rx_metadata_error_code(handle, out var ec), handle, uhd_rx_metadata_last_error); return ec; } }

    public RxMetadata() => Raise(uhd_rx_metadata_make(out handle));

    public void Dispose() => Raise(uhd_rx_metadata_free(ref handle));
}