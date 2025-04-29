using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NordicSpaceLink.LibUHD.NativeMethods;

namespace NordicSpaceLink.LibUHD;

public class TxStreamer : IDisposable
{
    private bool disposedValue;
    private uhd_tx_streamer_handle handle;

    internal ref uhd_tx_streamer_handle Handle => ref Unsafe.AsRef(ref handle);

    public TxStreamer()
    {
        Raise(uhd_tx_streamer_make(out handle), handle, uhd_tx_streamer_last_error);
    }

    public int NumberOfChannels
    {
        get
        {
            Raise(uhd_tx_streamer_num_channels(handle, out var result), handle, uhd_tx_streamer_last_error);
            return (int)result;
        }
    }

    public int MaxNumberOfSamps
    {
        get
        {
            Raise(uhd_tx_streamer_max_num_samps(handle, out var result), handle, uhd_tx_streamer_last_error);
            return (int)result;
        }
    }

    public unsafe int Send<T>(ReadOnlySpan<T> buffer, int samplesPerBuffer, TxMetadata metadata, double timeout = 0.1) where T : unmanaged
    {
        IntPtr* buffPtr = stackalloc IntPtr[1];

        fixed (T* ptr = buffer)
        {
            buffPtr[0] = new IntPtr(ptr);
            Raise(uhd_tx_streamer_send(handle, ref buffPtr[0], (uint)samplesPerBuffer, ref metadata.Handle, timeout, out var items_sent), handle, uhd_tx_streamer_last_error);
            return (int)items_sent;
        }
    }

    public bool ReceiveAsyncMessage(ref AsyncMetadata metadata, double timeout = 0.1)
    {
        var valid = false;
        Raise(uhd_tx_streamer_recv_async_msg(handle, ref metadata.Handle, timeout, ref valid), handle, uhd_tx_streamer_last_error);
        return valid;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            Raise(uhd_tx_streamer_free(ref handle));
            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~TxStreamer()
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