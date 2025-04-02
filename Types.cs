using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NordicSpaceLink.LibUHD.NativeMethods;

namespace NordicSpaceLink.LibUHD;

#pragma warning disable CS0649 // Field is never assigned to
enum uhd_error
{
    UHD_ERROR_NONE = 0,
    UHD_ERROR_INVALID_DEVICE = 1,
    UHD_ERROR_INDEX = 10,
    UHD_ERROR_KEY = 11,
    UHD_ERROR_NOT_IMPLEMENTED = 20,
    UHD_ERROR_USB = 21,
    UHD_ERROR_IO = 30,
    UHD_ERROR_OS = 31,
    UHD_ERROR_ASSERTION = 40,
    UHD_ERROR_LOOKUP = 41,
    UHD_ERROR_TYPE = 42,
    UHD_ERROR_VALUE = 43,
    UHD_ERROR_RUNTIME = 44,
    UHD_ERROR_ENVIRONMENT = 45,
    UHD_ERROR_SYSTEM = 46,
    UHD_ERROR_EXCEPT = 47,
    UHD_ERROR_BOOSTEXCEPT = 60,
    UHD_ERROR_STDEXCEPT = 70,
    UHD_ERROR_UNKNOWN = 100
}

struct uhd_rx_metadata_t;
struct uhd_tx_metadata_t;
struct uhd_async_metadata_t;

public enum RxMetadataErrorCode
{
    None = 0x0,
    Timeout = 0x1,
    LateCommand = 0x2,
    BrokenChain = 0x4,
    Overflow = 0x8,
    Alignment = 0xC,
    BadPacket = 0xF
}

public enum AsyncMetadataEventCode
{
    BurstAck = 0x1,
    Underflow = 0x2,
    SeqError = 0x4,
    TimeError = 0x8,
    UnderflowInPacket = 0x10,
    SeqErrorInBurst = 0x20,
    UserPayload = 0x40
}

struct uhd_meta_range_t;

struct uhd_sensor_value_t;

enum uhd_sensor_value_data_type_t
{
    UHD_SENSOR_VALUE_BOOLEAN = 98,
    UHD_SENSOR_VALUE_INTEGER = 105,
    UHD_SENSOR_VALUE_REALNUM = 114,
    UHD_SENSOR_VALUE_STRING = 115
}


struct uhd_string_vector_t;


public enum TuneRequestPolicy
{
    None = 78,
    Auto = 65,
    Manual = 77
}

unsafe struct UhdTuneRequest
{
    public double target_freq;
    public TuneRequestPolicy rf_freq_policy;
    public double rf_freq;
    public TuneRequestPolicy dsp_freq_policy;
    public double dsp_freq;
    public char* args;

    public UhdTuneRequest(TuneRequest request)
    {
        target_freq = request.TargetFrequency;
        rf_freq_policy = request.RfFrequencyPolicy;
        rf_freq = request.RfFrequency;
        dsp_freq_policy = request.DspFrequencyPolicy;
        dsp_freq = request.DspFrequency;
        args = (char*)Marshal.StringToHGlobalAnsi(request.Args);
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal((nint)args);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct TuneResult
{
    public double ClippedRfFrequency;
    public double TargetRfFrequency;
    public double ActualRfFrequency;
    public double TargetDspFrequency;
    public double ActualDspFrequency;
}

public readonly record struct RxInfo(string MboardId, string MboardName, string MboardSerial, string RxId, string RxSubdevName, string RxSubdevSpec, string RxSerial, string RxAntenna)
{
}

public readonly record struct TxInfo(string MboardId, string MboardName, string MboardSerial, string TxId, string TxSubdevName, string TxSubdevSpec, string TxSerial, string TxAntenna)
{
}

[StructLayout(LayoutKind.Sequential)]
unsafe struct uhd_usrp_rx_info_t
{
    public char* mboard_id;
    public char* mboard_name;
    public char* mboard_serial;
    public char* rx_id;
    public char* rx_subdev_name;
    public char* rx_subdev_spec;
    public char* rx_serial;
    public char* rx_antenna;

    public readonly RxInfo Info()
    {
        return new(
            new(mboard_id),
            new(mboard_name),
            new(mboard_serial),
            new(rx_id),
            new(rx_subdev_name),
            new(rx_subdev_spec),
            new(rx_serial),
            new(rx_antenna)
        );
    }
}

[StructLayout(LayoutKind.Sequential)]
unsafe struct uhd_usrp_tx_info_t
{
    public char* mboard_id;
    public char* mboard_name;
    public char* mboard_serial;
    public char* tx_id;
    public char* tx_subdev_name;
    public char* tx_subdev_spec;
    public char* tx_serial;
    public char* tx_antenna;

    public readonly TxInfo Info()
    {
        return new(
            new(mboard_id),
            new(mboard_name),
            new(mboard_serial),
            new(tx_id),
            new(tx_subdev_name),
            new(tx_subdev_spec),
            new(tx_serial),
            new(tx_antenna)
        );
    }
}

struct uhd_dboard_eeprom_t;

struct uhd_mboard_eeprom_t;

[StructLayout(LayoutKind.Sequential)]
unsafe struct uhd_subdev_spec_pair_t
{
    internal char* db_name;
    internal char* sd_name;
}

struct uhd_subdev_spec_t;

[StructLayout(LayoutKind.Sequential)]
struct uhd_usrp_register_info_t
{
    internal nuint bitwidth;
    internal bool readable;
    internal bool writable;
}

[StructLayout(LayoutKind.Sequential)]
unsafe struct UhdStreamArgs
{
    readonly char* cpu_format;
    readonly char* otw_format;
    readonly char* args;
    readonly nuint* channel_list;
    readonly int n_channels;

    public UhdStreamArgs(StreamArgs request)
    {
        n_channels = request.Channels.Count;
        channel_list = (nuint*)Marshal.AllocHGlobal(Unsafe.SizeOf<nuint>() * request.Channels.Count);
        for (int i = 0; i < request.Channels.Count; i++)
            channel_list[i] = (nuint)request.Channels[i];

        cpu_format = (char*)Marshal.StringToHGlobalAnsi(request.CpuFormat);
        otw_format = (char*)Marshal.StringToHGlobalAnsi(request.OtwFormat);
        args = (char*)Marshal.StringToHGlobalAnsi(request.Args);
    }

    public readonly void Dispose()
    {
        Marshal.FreeHGlobal((nint)channel_list);
        Marshal.FreeHGlobal((nint)cpu_format);
        Marshal.FreeHGlobal((nint)otw_format);
        Marshal.FreeHGlobal((nint)args);
    }
}

public enum StreamMode
{
    StartContinuous = 97,
    StopContinuous = 111,
    NumSampsAndDone = 100,
    NumSampsAndMode = 109,
}

[StructLayout(LayoutKind.Sequential)]
public struct TimeSpec(long fullSeconds, double fracSeconds)
{
    public long FullSeconds = fullSeconds;
    public double FracSeconds = fracSeconds;
}

[StructLayout(LayoutKind.Sequential)]
public struct StreamCommand
{
    public StreamMode StreamMode;
    public nuint NumberOfSamples;
    public bool StreamNow;
    public TimeSpec TimeSpecification;
}

struct uhd_rx_streamer;
struct uhd_tx_streamer;
struct uhd_usrp;

struct uhd_usrp_clock;

enum uhd_log_severity_level_t
{
    UHD_LOG_LEVEL_TRACE,
    UHD_LOG_LEVEL_DEBUG,
    UHD_LOG_LEVEL_INFO,
    UHD_LOG_LEVEL_WARNING,
    UHD_LOG_LEVEL_ERROR,
    UHD_LOG_LEVEL_FATAL
}

[InlineArray(4)]
public struct UserPayload
{
    uint value;
}

[StructLayout(LayoutKind.Sequential)]
struct Handle<T>
{
    IntPtr value;
}

struct uhd_rx_metadata_handle { internal Handle<uhd_rx_metadata_t> handle; }
struct uhd_tx_metadata_handle { internal Handle<uhd_tx_metadata_t> handle; }
struct uhd_async_metadata_handle { internal Handle<uhd_async_metadata_t> handle; }
struct uhd_meta_range_handle { internal Handle<uhd_meta_range_t> handle; }
struct uhd_sensor_value_handle { internal Handle<uhd_sensor_value_t> handle; }
struct uhd_dboard_eeprom_handle { internal Handle<uhd_dboard_eeprom_t> handle; }
struct uhd_mboard_eeprom_handle { internal Handle<uhd_mboard_eeprom_t> handle; }
struct uhd_subdev_spec_handle { internal Handle<uhd_subdev_spec_t> handle; }
struct uhd_rx_streamer_handle { internal Handle<uhd_rx_streamer> handle; }
struct uhd_tx_streamer_handle { internal Handle<uhd_tx_streamer> handle; }
struct uhd_usrp_handle { internal Handle<uhd_usrp> handle; }
struct uhd_usrp_clock_handle { internal Handle<uhd_usrp_clock> handle; }
struct uhd_string_vector_handle { internal Handle<uhd_string_vector_t> handle; }

public readonly record struct SubdevSpec(string Str, string PP)
{
}

internal ref struct SubdevSpecHandle
{
    private uhd_subdev_spec_handle handle;

    public ref uhd_subdev_spec_handle Handle => ref Unsafe.AsRef(ref handle);

    public readonly SubdevSpec Subdev
    {
        get
        {
            using var buffer = new Pooled<char>(1024);
            Raise(uhd_subdev_spec_to_pp_string(handle, buffer.Buffer, buffer.Length));
            var pp = new string(buffer.Buffer);

            Raise(uhd_subdev_spec_to_string(handle, buffer.Buffer, buffer.Length));
            var str = new string(buffer.Buffer);

            return new SubdevSpec(str, pp);
        }
    }

    public SubdevSpecHandle(string markup)
    {
        Raise(uhd_subdev_spec_make(out handle, markup));
    }

    public void Dispose()
    {
        Raise(uhd_subdev_spec_free(ref handle));
    }
}

ref struct StringVector
{
    private uhd_string_vector_handle handle;

    public StringVector()
    {
        Raise(uhd_string_vector_make(out handle));
    }

    public readonly int Length
    {
        get
        {
            Raise(uhd_string_vector_size(handle, out var size));
            return (int)size;
        }
    }

    public ref uhd_string_vector_handle Handle => ref Unsafe.AsRef(ref handle);

    public readonly string this[int index]
    {
        get
        {
            using var buffer = new Pooled<char>(1024);

            Raise(uhd_string_vector_at(handle, (nuint)index, buffer.Buffer, buffer.Length));
            return new string(buffer.Buffer);
        }
    }

    public void Add(string value)
    {
        Raise(uhd_string_vector_push_back(ref handle, value));
    }

    public void Dispose()
    {
        Raise(uhd_string_vector_free(ref handle));
    }

    public readonly string[] ToArray()
    {
        var result = new string[Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = this[i];
        return result;
    }
}

internal ref struct Pooled<T>(int size)
{
    private readonly T[] buffer = ArrayPool<T>.Shared.Rent(size);
    public readonly int Length => size;
    public readonly T[] Buffer => buffer;
    public void Dispose() => ArrayPool<T>.Shared.Return(buffer);
}
#pragma warning restore CS0649 // Field is never assigned to