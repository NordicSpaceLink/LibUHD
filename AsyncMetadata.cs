using System.Runtime.CompilerServices;
using static NordicSpaceLink.LibUHD.NativeMethods;

namespace NordicSpaceLink.LibUHD;

public ref struct AsyncMetadata
{
    private uhd_async_metadata_handle handle;

    internal ref uhd_async_metadata_handle Handle => ref Unsafe.AsRef(ref handle);

    public readonly bool HasTimeSpec { get { Raise(uhd_async_metadata_has_time_spec(handle, out var result), handle, uhd_async_metadata_last_error); return result; } }
    public readonly AsyncMetadataEventCode EventCode { get { Raise(uhd_async_metadata_event_code(handle, out var result), handle, uhd_async_metadata_last_error); return result; } }
    public readonly nuint Channel { get { Raise(uhd_async_metadata_channel(handle, out var result), handle, uhd_async_metadata_last_error); return result; } }
    public readonly UserPayload @UserPayload { get { Raise(uhd_async_metadata_user_payload(handle, out var result), handle, uhd_async_metadata_last_error); return result; } }
    public readonly (long full_secs_out, double frac_secs_out) TimeSpec { get { Raise(uhd_async_metadata_time_spec(handle, out var a, out var b), handle, uhd_async_metadata_last_error); return (a, b); } }


    // void last_error([Out] char[] error_out, nint strbuffer_len) { }

    public AsyncMetadata() => Raise(uhd_async_metadata_make(out handle));

    public void Dispose() => Raise(uhd_async_metadata_free(ref handle));
}