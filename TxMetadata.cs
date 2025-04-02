using System.Runtime.CompilerServices;
using static NordicSpaceLink.LibUHD.NativeMethods;

namespace NordicSpaceLink.LibUHD;

public ref struct TxMetadata
{
    private uhd_tx_metadata_handle handle;

    internal ref uhd_tx_metadata_handle Handle => ref Unsafe.AsRef(ref handle);

    public readonly bool HasTimeSpec { get { Raise(uhd_tx_metadata_has_time_spec(handle, out var result), handle, uhd_tx_metadata_last_error); return result; } }
    public readonly bool StartOfBurst { get { Raise(uhd_tx_metadata_start_of_burst(handle, out var result), handle, uhd_tx_metadata_last_error); return result; } }
    public readonly bool EndOfBurst { get { Raise(uhd_tx_metadata_end_of_burst(handle, out var result), handle, uhd_tx_metadata_last_error); return result; } }
    public readonly (long full_secs_out, double frac_secs_out) TimeSpec { get { Raise(uhd_tx_metadata_time_spec(handle, out var a, out var b), handle, uhd_tx_metadata_last_error); return (a, b); } }

    public TxMetadata(bool has_time_spec, long full_secs, double frac_secs, bool start_of_burst, bool end_of_burst) => Raise(uhd_tx_metadata_make(out handle, has_time_spec, full_secs, frac_secs, start_of_burst, end_of_burst));
    public TxMetadata() : this(false, 0, 0, false ,false)
    {
    }

    public void Dispose() => Raise(uhd_tx_metadata_free(ref handle));
}