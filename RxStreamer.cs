using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NordicSpaceLink.LibUHD.NativeMethods;

namespace NordicSpaceLink.LibUHD;

public class RxStreamer : IDisposable
{
    private bool disposedValue;
    private uhd_rx_streamer_handle handle;

    internal ref uhd_rx_streamer_handle Handle => ref Unsafe.AsRef(ref handle);

    public RxStreamer()
    {
        Raise(uhd_rx_streamer_make(out handle));
    }

    public int NumberOfChannels
    {
        get
        {
            Raise(uhd_rx_streamer_num_channels(handle, out var result), handle, uhd_rx_streamer_last_error);
            return (int)result;
        }
    }

    public int MaxNumberOfSamps
    {
        get
        {
            Raise(uhd_rx_streamer_max_num_samps(handle, out var result), handle, uhd_rx_streamer_last_error);
            return (int)result;
        }
    }

    public unsafe int Receive<T>(Span<T> buffer, int samplesPerBuffer, RxMetadata metadata, double timeout = 0.1, bool onePacket = false) where T : unmanaged
    {
        IntPtr* buffPtr = stackalloc IntPtr[1];
        fixed (T* ptr = buffer)
        {
            buffPtr[0] = new IntPtr(ptr);

            Raise(uhd_rx_streamer_recv(handle, ref buffPtr[0], (uint)samplesPerBuffer, ref metadata.Handle, timeout, onePacket, out var items_recvd), handle, uhd_rx_streamer_last_error);
            return (int)items_recvd;
        }
    }

    public void IssueStreamCommand(StreamCommand command)
    {
        Raise(uhd_rx_streamer_issue_stream_cmd(handle, ref command), handle, uhd_rx_streamer_last_error);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            Raise(uhd_rx_streamer_free(ref handle));
            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~RxStreamer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}