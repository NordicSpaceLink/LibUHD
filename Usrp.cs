using System;
using System.Collections.Generic;
using static NordicSpaceLink.LibUHD.NativeMethods;

namespace NordicSpaceLink.LibUHD;

public struct TuneRequest
{
    public TuneRequest(double targetFrequency = 0)
    {
        TargetFrequency = targetFrequency;
    }

    public TuneRequest(double targetFrequency, double loOffset)
    {
        TargetFrequency = targetFrequency;
        RfFrequencyPolicy = TuneRequestPolicy.Manual;
        RfFrequency = targetFrequency + loOffset;
    }

    public double TargetFrequency;
    public TuneRequestPolicy RfFrequencyPolicy = TuneRequestPolicy.Auto;
    public double RfFrequency = 0;
    public TuneRequestPolicy DspFrequencyPolicy = TuneRequestPolicy.Auto;
    public double DspFrequency = 0;
    public string Args = "";
}

public struct StreamArgs(string cpu = "", string otw = "")
{
    public string CpuFormat = cpu;
    public string OtwFormat = otw;
    public string Args = "";
    public readonly List<int> Channels = [];
}

public class USRP : IDisposable
{
    public const int ALL_MBOARDS = -1;
    public const int ALL_CHANS = -1;
    public const string ALL_GAINS = "";
    public const string ALL_LOS = "all";

    private uhd_usrp_handle handle;
    private bool disposedValue;

    public USRP(string args = "")
    {
        Raise(uhd_usrp_make(out handle, args));
    }

    public RxStreamer GetRXStream(StreamArgs streamArgs)
    {
        RxStreamer streamer = new();
        var args = new UhdStreamArgs(streamArgs);
        try
        {
            Raise(uhd_usrp_get_rx_stream(handle, ref args, streamer.Handle), handle, uhd_usrp_last_error);
            return streamer;
        }
        finally
        {
            args.Dispose();
        }
    }

    public TxStreamer GetTXStream(StreamArgs streamArgs)
    {
        TxStreamer streamer = new();
        var args = new UhdStreamArgs(streamArgs);
        try
        {
            Raise(uhd_usrp_get_tx_stream(handle, ref args, streamer.Handle), handle, uhd_usrp_last_error);
            return streamer;
        }
        finally
        {
            args.Dispose();
        }
    }

    public RxInfo GetRxInfo(int chan = 0)
    {
        Raise(uhd_usrp_get_rx_info(handle, (uint)chan, out var info), handle, uhd_usrp_last_error);
        try
        {
            return info.Info();
        }
        finally
        {
            Raise(uhd_usrp_rx_info_free(ref info), handle, uhd_usrp_last_error);
        }
    }
    public TxInfo GetTxInfo(int chan = 0)
    {
        Raise(uhd_usrp_get_tx_info(handle, (uint)chan, out var info), handle, uhd_usrp_last_error);
        try
        {
            return info.Info();
        }
        finally
        {
            Raise(uhd_usrp_tx_info_free(ref info), handle, uhd_usrp_last_error);
        }
    }

    public void SetMasterClockRate(double rate, int mboard = ALL_MBOARDS) => Raise(uhd_usrp_set_master_clock_rate(handle, rate, (uint)mboard), handle, uhd_usrp_last_error);
    public double GetMasterClockRate(int mboard = 0) { Raise(uhd_usrp_get_master_clock_rate(handle, (uint)mboard, out var rate), handle, uhd_usrp_last_error); return rate; }

    public string PPString
    {
        get
        {
            using var buf = new Pooled<char>(1024);
            Raise(uhd_usrp_get_pp_string(handle, buf.Buffer, buf.Length), handle, uhd_usrp_last_error);
            return new(buf.Buffer);
        }
    }

    public string GetMBoardName(int mboard = 0)
    {
        using var buf = new Pooled<char>(1024);
        Raise(uhd_usrp_get_mboard_name(handle, (uint)mboard, buf.Buffer, buf.Length), handle, uhd_usrp_last_error);
        return new(buf.Buffer);
    }

    public TimeSpec GetTimeNow(int mboard = 0)
    {
        Raise(uhd_usrp_get_time_now(handle, (uint)mboard, out var full_secs_out, out var frac_secs_out), handle, uhd_usrp_last_error);
        return new(full_secs_out, frac_secs_out);
    }

    public TimeSpec GetTimeLastPPS(int mboard = 0)
    {
        Raise(uhd_usrp_get_time_last_pps(handle, (uint)mboard, out var full_secs_out, out var frac_secs_out), handle, uhd_usrp_last_error);
        return new(full_secs_out, frac_secs_out);
    }

    public void SetTimeNow(TimeSpec timeSpec, int mboard = ALL_MBOARDS)
    {
        Raise(uhd_usrp_set_time_now(handle, timeSpec.FullSeconds, timeSpec.FracSeconds, (uint)mboard), handle, uhd_usrp_last_error);
    }

    public void SetTimeNextPPS(TimeSpec timeSpec, int mboard = ALL_MBOARDS)
    {
        Raise(uhd_usrp_set_time_next_pps(handle, timeSpec.FullSeconds, timeSpec.FracSeconds, (uint)mboard), handle, uhd_usrp_last_error);
    }

    public void SetTimeUnknownPPS(TimeSpec timeSpec)
    {
        Raise(uhd_usrp_set_time_unknown_pps(handle, timeSpec.FullSeconds, timeSpec.FracSeconds), handle, uhd_usrp_last_error);
    }

    public bool IsTimeSynchronized
    {
        get
        {
            Raise(uhd_usrp_get_time_synchronized(handle, out var result), handle, uhd_usrp_last_error);
            return result;
        }
    }

    public void SetCommandTime(TimeSpec timeSpec, int mboard = ALL_MBOARDS)
    {
        Raise(uhd_usrp_set_command_time(handle, timeSpec.FullSeconds, timeSpec.FracSeconds, (uint)mboard), handle, uhd_usrp_last_error);
    }

    public void ClearCommandTime(int mboard = ALL_MBOARDS)
    {
        Raise(uhd_usrp_clear_command_time(handle, (uint)mboard), handle, uhd_usrp_last_error);
    }

    public void SetTimeSource(string timeSource, int mboard = ALL_MBOARDS)
    {
        Raise(uhd_usrp_set_time_source(handle, timeSource, (uint)mboard), handle, uhd_usrp_last_error);
    }

    public string[] GetTimeSources(int mboard)
    {
        using var sv = new StringVector();
        Raise(uhd_usrp_get_time_sources(handle, (uint)mboard, ref sv.Handle), handle, uhd_usrp_last_error);
        return sv.ToArray();
    }

    public string GetTimeSource(int mboard)
    {
        using var buf = new Pooled<char>(1024);
        Raise(uhd_usrp_get_time_source(handle, (uint)mboard, buf.Buffer, buf.Length), handle, uhd_usrp_last_error);
        return new string(buf.Buffer);
    }

    public void SetClockSource(string timeSource, int mboard = ALL_MBOARDS)
    {
        Raise(uhd_usrp_set_clock_source(handle, timeSource, (uint)mboard), handle, uhd_usrp_last_error);
    }

    public string GetClockSource(int mboard)
    {
        using var buf = new Pooled<char>(1024);
        Raise(uhd_usrp_get_clock_source(handle, (uint)mboard, buf.Buffer, buf.Length), handle, uhd_usrp_last_error);
        return new string(buf.Buffer);
    }

    public string[] GetClockSources(int mboard)
    {
        using var sv = new StringVector();
        Raise(uhd_usrp_get_clock_sources(handle, (uint)mboard, ref sv.Handle), handle, uhd_usrp_last_error);
        return sv.ToArray();
    }

    public void SetClockSourceOut(bool enable, int mboard = ALL_MBOARDS)
    {
        Raise(uhd_usrp_set_clock_source_out(handle, enable, (uint)mboard), handle, uhd_usrp_last_error);
    }

    public void SetTimeSourceOut(bool enable, int mboard = ALL_MBOARDS)
    {
        Raise(uhd_usrp_set_time_source_out(handle, enable, (uint)mboard), handle, uhd_usrp_last_error);
    }

    public int MBoardCount
    {
        get
        {
            Raise(uhd_usrp_get_num_mboards(handle, out var result), handle, uhd_usrp_last_error);
            return (int)result;
        }
    }

    public SensorValue GetMboardSensor(string name, int mboard = 0) { using var sv = new SensorValueHandle(); Raise(uhd_usrp_get_mboard_sensor(handle, name, (uint)mboard, ref sv.Handle), handle, uhd_usrp_last_error); return sv.Value; }
    public string[] GetMboardSensorNames(int mboard = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_mboard_sensor_names(handle, (uint)mboard, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }

    // uhd_usrp_set_user_register(uhd_usrp_handle h, byte addr, uint data, int mboard);
    // uhd_usrp_get_mboard_eeprom(uhd_usrp_handle h, uhd_mboard_eeprom_handle mb_eeprom, int mboard);
    // uhd_usrp_set_mboard_eeprom(uhd_usrp_handle h, uhd_mboard_eeprom_handle mb_eeprom, int mboard);
    // uhd_usrp_get_dboard_eeprom(uhd_usrp_handle h, uhd_dboard_eeprom_handle db_eeprom, string unit, string slot, int mboard);
    // uhd_usrp_set_dboard_eeprom(uhd_usrp_handle h, uhd_dboard_eeprom_handle db_eeprom, string unit, string slot, int mboard);

    public SubdevSpec GetRxSubdevSpec(int mboard = 0) { using var ss = new SubdevSpecHandle(""); Raise(uhd_usrp_get_rx_subdev_spec(handle, (uint)mboard, ss.Handle), handle, uhd_usrp_last_error); return ss.Subdev; }
    public int GetRxNumChannels() { Raise(uhd_usrp_get_rx_num_channels(handle, out nuint num_channels_out), handle, uhd_usrp_last_error); return (int)num_channels_out; }
    public double GetRxRate(int chan = 0) { Raise(uhd_usrp_get_rx_rate(handle, (nuint)chan, out var rateOut), handle, uhd_usrp_last_error); return rateOut; }
    public MetaRange GetRxRates(int chan = 0) { using var x = new MetaRangeBuf(); Raise(uhd_usrp_get_rx_rates(handle, (nuint)chan, x.Handle), handle, uhd_usrp_last_error); return x.Build(); }
    public double GetRxFreq(int chan = 0) { Raise(uhd_usrp_get_rx_freq(handle, (nuint)chan, out var freqOut), handle, uhd_usrp_last_error); return freqOut; }
    public MetaRange GetRxFreqRange(int chan = 0) { using var x = new MetaRangeBuf(); Raise(uhd_usrp_get_rx_freq_range(handle, (nuint)chan, x.Handle), handle, uhd_usrp_last_error); return x.Build(); }
    public MetaRange GetFeRxFreqRange(int chan = 0) { using var x = new MetaRangeBuf(); Raise(uhd_usrp_get_fe_rx_freq_range(handle, (nuint)chan, x.Handle), handle, uhd_usrp_last_error); return x.Build(); }
    public bool GetRxLoExportEnabled(string name, int chan = 0) { Raise(uhd_usrp_get_rx_lo_export_enabled(handle, name, (nuint)chan, out var resultOut), handle, uhd_usrp_last_error); return resultOut; }
    public double GetRxLoFreq(string name, int chan = 0) { Raise(uhd_usrp_get_rx_lo_freq(handle, name, (nuint)chan, out var rxLoFreqOut), handle, uhd_usrp_last_error); return rxLoFreqOut; }
    public double GetRxGain(string gainName = ALL_GAINS, int chan = 0) { Raise(uhd_usrp_get_rx_gain(handle, (nuint)chan, gainName, out var gainOut), handle, uhd_usrp_last_error); return gainOut; }
    public double GetNormalizedRxGain(int chan = 0) { Raise(uhd_usrp_get_normalized_rx_gain(handle, (nuint)chan, out var gainOut), handle, uhd_usrp_last_error); return gainOut; }
    public MetaRange GetRxGainRange(string name = ALL_GAINS, int chan = 0) { using var x = new MetaRangeBuf(); Raise(uhd_usrp_get_rx_gain_range(handle, name, (nuint)chan, x.Handle), handle, uhd_usrp_last_error); return x.Build(); }
    public string[] GetRxLoNames(int chan = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_rx_lo_names(handle, (nuint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }
    public string[] GetRxLoSources(string name, int chan = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_rx_lo_sources(handle, name, (nuint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }
    public string[] GetRxGainNames(int chan = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_rx_gain_names(handle, (nuint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }
    public string[] GetRxAntennas(int chan = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_rx_antennas(handle, (nuint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }
    public double GetRxBandwidth(int chan = 0) { Raise(uhd_usrp_get_rx_bandwidth(handle, (nuint)chan, out var bandwidthOut), handle, uhd_usrp_last_error); return bandwidthOut; }
    public MetaRange GetRxBandwidthRange(int chan = 0) { using var x = new MetaRangeBuf(); Raise(uhd_usrp_get_rx_bandwidth_range(handle, (nuint)chan, x.Handle), handle, uhd_usrp_last_error); return x.Build(); }
    public SensorValue GetRxSensor(string name, int chan = 0) { using var sv = new SensorValueHandle(); Raise(uhd_usrp_get_rx_sensor(handle, name, (uint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.Value; }
    public string[] GetRxSensorNames(int chan = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_rx_sensor_names(handle, (nuint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }
    public string GetRxSubdevName(int chan = 0) { using var buf = new Pooled<char>(1024); Raise(uhd_usrp_get_rx_subdev_name(handle, (nuint)chan, buf.Buffer, buf.Length), handle, uhd_usrp_last_error); return new string(buf.Buffer); }
    public string GetRxLoSource(string name, int chan = 0) { using var buf = new Pooled<char>(1024); Raise(uhd_usrp_get_rx_lo_source(handle, name, (nuint)chan, buf.Buffer, buf.Length), handle, uhd_usrp_last_error); return new string(buf.Buffer); }
    public string GetRxAntenna(int chan = 0) { using var buf = new Pooled<char>(1024); Raise(uhd_usrp_get_rx_antenna(handle, (nuint)chan, buf.Buffer, buf.Length), handle, uhd_usrp_last_error); return new string(buf.Buffer); }
    public void SetRxSubdevSpec(string subdevSpec, int mboard = ALL_MBOARDS) { using var ss = new SubdevSpecHandle(subdevSpec); Raise(uhd_usrp_set_rx_subdev_spec(handle, ss.Handle, (uint)mboard), handle, uhd_usrp_last_error); }
    public void SetRxRate(double rate, int chan = ALL_CHANS) { Raise(uhd_usrp_set_rx_rate(handle, rate, (nuint)chan), handle, uhd_usrp_last_error); }
    public TuneResult SetRxFreq(TuneRequest tuneRequest, int chan = 0) { var req = new UhdTuneRequest(tuneRequest); try { Raise(uhd_usrp_set_rx_freq(handle, ref req, (nuint)chan, out var tuneResult), handle, uhd_usrp_last_error); return tuneResult; } finally { req.Dispose(); } }
    public void SetRxLoSource(string src, string name, int chan = 0) { Raise(uhd_usrp_set_rx_lo_source(handle, src, name, (nuint)chan), handle, uhd_usrp_last_error); }
    public void SetRxLoExportEnabled(bool enabled, string name, int chan = 0) { Raise(uhd_usrp_set_rx_lo_export_enabled(handle, enabled, name, (nuint)chan), handle, uhd_usrp_last_error); }
    public double SetRxLoFreq(double freq, string name, int chan = 0) { Raise(uhd_usrp_set_rx_lo_freq(handle, freq, name, (nuint)chan, out var coercedFreqOut), handle, uhd_usrp_last_error); return coercedFreqOut; }
    public void SetRxGain(double gain, string gainName = ALL_GAINS, int chan = 0) { Raise(uhd_usrp_set_rx_gain(handle, gain, (nuint)chan, gainName), handle, uhd_usrp_last_error); }
    public void SetNormalizedRxGain(double gain, int chan = 0) { Raise(uhd_usrp_set_normalized_rx_gain(handle, gain, (nuint)chan), handle, uhd_usrp_last_error); }
    public void SetRxAgc(bool enable, int chan = 0) { Raise(uhd_usrp_set_rx_agc(handle, enable, (nuint)chan), handle, uhd_usrp_last_error); }
    public void SetRxAntenna(string ant, int chan = 0) { Raise(uhd_usrp_set_rx_antenna(handle, ant, (nuint)chan), handle, uhd_usrp_last_error); }
    public void SetRxBandwidth(double bandwidth, int chan = 0) { Raise(uhd_usrp_set_rx_bandwidth(handle, bandwidth, (nuint)chan), handle, uhd_usrp_last_error); }
    public void SetRxDcOffsetEnabled(bool enb, int chan = ALL_CHANS) { Raise(uhd_usrp_set_rx_dc_offset_enabled(handle, enb, (nuint)chan), handle, uhd_usrp_last_error); }
    public void SetRxIqBalanceEnabled(bool enb, int chan) { Raise(uhd_usrp_set_rx_iq_balance_enabled(handle, enb, (nuint)chan), handle, uhd_usrp_last_error); }

    public SubdevSpec GetTxSubdevSpec(int mboard = 0) { using var ss = new SubdevSpecHandle(""); Raise(uhd_usrp_get_tx_subdev_spec(handle, (uint)mboard, ss.Handle), handle, uhd_usrp_last_error); return ss.Subdev; }
    public int GetTxNumChannels() { Raise(uhd_usrp_get_tx_num_channels(handle, out var numChannelsOut), handle, uhd_usrp_last_error); return (int)numChannelsOut; }
    public double GetTxRate(int chan = 0) { Raise(uhd_usrp_get_tx_rate(handle, (nuint)chan, out var rateOut), handle, uhd_usrp_last_error); return rateOut; }
    public MetaRange GetTxRates(int chan = 0) { using var x = new MetaRangeBuf(); Raise(uhd_usrp_get_tx_rates(handle, (nuint)chan, x.Handle), handle, uhd_usrp_last_error); return x.Build(); }
    public double GetTxFreq(int chan = 0) { Raise(uhd_usrp_get_tx_freq(handle, (nuint)chan, out var freqOut), handle, uhd_usrp_last_error); return freqOut; }
    public MetaRange GetTxFreqRange(int chan = 0) { using var x = new MetaRangeBuf(); Raise(uhd_usrp_get_tx_freq_range(handle, (nuint)chan, x.Handle), handle, uhd_usrp_last_error); return x.Build(); }
    public MetaRange GetFeTxFreqRange(int chan = 0) { using var x = new MetaRangeBuf(); Raise(uhd_usrp_get_fe_tx_freq_range(handle, (nuint)chan, x.Handle), handle, uhd_usrp_last_error); return x.Build(); }
    public bool GetTxLoExportEnabled(string name, int chan = 0) { Raise(uhd_usrp_get_tx_lo_export_enabled(handle, name, (nuint)chan, out var resultOut), handle, uhd_usrp_last_error); return resultOut; }
    public double GetTxLoFreq(string name, int chan = 0) { Raise(uhd_usrp_get_tx_lo_freq(handle, name, (nuint)chan, out var txLoFreqOut), handle, uhd_usrp_last_error); return txLoFreqOut; }
    public MetaRange GetTxGainRange(string name = ALL_GAINS, int chan = 0) { using var x = new MetaRangeBuf(); Raise(uhd_usrp_get_tx_gain_range(handle, name, (nuint)chan, x.Handle), handle, uhd_usrp_last_error); return x.Build(); }
    public double GetTxGain(string gainName = ALL_GAINS, int chan = 0) { Raise(uhd_usrp_get_tx_gain(handle, (nuint)chan, gainName, out var gainOut), handle, uhd_usrp_last_error); return gainOut; }
    public double GetNormalizedTxGain(int chan = 0) { Raise(uhd_usrp_get_normalized_tx_gain(handle, (nuint)chan, out var gainOut), handle, uhd_usrp_last_error); return gainOut; }
    public double GetTxBandwidth(int chan = 0) { Raise(uhd_usrp_get_tx_bandwidth(handle, (nuint)chan, out var bandwidth_out), handle, uhd_usrp_last_error); return bandwidth_out; }
    public MetaRange GetTxBandwidthRange(int chan = 0) { using var x = new MetaRangeBuf(); Raise(uhd_usrp_get_tx_bandwidth_range(handle, (nuint)chan, x.Handle), handle, uhd_usrp_last_error); return x.Build(); }
    public SensorValue GetTxSensor(string name, int chan = 0) { using var sv = new SensorValueHandle(); Raise(uhd_usrp_get_tx_sensor(handle, name, (uint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.Value; }
    public string[] GetTxSensorNames(int chan = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_tx_sensor_names(handle, (nuint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }
    public string[] GetTxLoNames(int chan = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_tx_lo_names(handle, (nuint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }
    public string[] GetTxLoSources(string name, int chan = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_tx_lo_sources(handle, name, (nuint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }
    public string[] GetTxGainNames(int chan = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_tx_gain_names(handle, (nuint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }
    public string[] GetTxAntennas(int chan = 0) { using var sv = new StringVector(); Raise(uhd_usrp_get_tx_antennas(handle, (nuint)chan, ref sv.Handle), handle, uhd_usrp_last_error); return sv.ToArray(); }
    public string GetTxSubdevName(int chan = 0) { using var buf = new Pooled<char>(1024); Raise(uhd_usrp_get_tx_subdev_name(handle, (nuint)chan, buf.Buffer, buf.Length), handle, uhd_usrp_last_error); return new(buf.Buffer); }
    public string GetTxLoSource(string name, int chan = 0) { using var buf = new Pooled<char>(1024); Raise(uhd_usrp_get_tx_lo_source(handle, name, (nuint)chan, buf.Buffer, buf.Length), handle, uhd_usrp_last_error); return new(buf.Buffer); }
    public string GetTxAntenna(int chan = 0) { using var buf = new Pooled<char>(1024); Raise(uhd_usrp_get_tx_antenna(handle, (nuint)chan, buf.Buffer, buf.Length), handle, uhd_usrp_last_error); return new(buf.Buffer); }
    public void SetTxSubdevSpec(string subdevSpec, int mboard = ALL_MBOARDS) { using var ss = new SubdevSpecHandle(subdevSpec); Raise(uhd_usrp_set_tx_subdev_spec(handle, ss.Handle, (uint)mboard), handle, uhd_usrp_last_error); }
    public void SetTxRate(double rate, int chan = ALL_CHANS) { Raise(uhd_usrp_set_tx_rate(handle, rate, (nuint)chan), handle, uhd_usrp_last_error); }
    public TuneResult SetTxFreq(TuneRequest tuneRequest, int chan = 0) { var req = new UhdTuneRequest(tuneRequest); try { Raise(uhd_usrp_set_tx_freq(handle, ref req, (nuint)chan, out var tuneResult), handle, uhd_usrp_last_error); return tuneResult; } finally { req.Dispose(); } }
    public void SetTxLoSource(string src, string name, int chan = 0) { Raise(uhd_usrp_set_tx_lo_source(handle, src, name, (nuint)chan), handle, uhd_usrp_last_error); }
    public void SetTxLoExportEnabled(bool enabled, string name, int chan = 0) { Raise(uhd_usrp_set_tx_lo_export_enabled(handle, enabled, name, (nuint)chan), handle, uhd_usrp_last_error); }
    public double SetTxLoFreq(double freq, string name, int chan = 0) { Raise(uhd_usrp_set_tx_lo_freq(handle, freq, name, (nuint)chan, out var coercedFreqOut), handle, uhd_usrp_last_error); return coercedFreqOut; }
    public void SetTxGain(double gain, string gainName = ALL_GAINS, int chan = 0) { Raise(uhd_usrp_set_tx_gain(handle, gain, (nuint)chan, gainName), handle, uhd_usrp_last_error); }
    public void SetNormalizedTxGain(double gain, int chan = 0) { Raise(uhd_usrp_set_normalized_tx_gain(handle, gain, (nuint)chan), handle, uhd_usrp_last_error); }
    public void SetTxAntenna(string ant, int chan = 0) { Raise(uhd_usrp_set_tx_antenna(handle, ant, (nuint)chan), handle, uhd_usrp_last_error); }
    public void SetTxBandwidth(double bandwidth, int chan = 0) { Raise(uhd_usrp_set_tx_bandwidth(handle, bandwidth, (nuint)chan), handle, uhd_usrp_last_error); }

    // uhd_usrp_get_gpio_banks(uhd_usrp_handle h, int mboard, ref uhd_string_vector_handle gpio_banks_out);
    // uhd_usrp_set_gpio_attr(uhd_usrp_handle h, string bank, string attr, uint value, uint mask, int mboard);
    // uhd_usrp_get_gpio_attr(uhd_usrp_handle h, string bank, string attr, int mboard, out uint attr_out);

    // uhd_usrp_enumerate_registers(uhd_usrp_handle h, int mboard, ref uhd_string_vector_handle registers_out);
    // uhd_usrp_get_register_info(uhd_usrp_handle h, string path, int mboard, out uhd_usrp_register_info_t register_info_out);
    // uhd_usrp_write_register(uhd_usrp_handle h, string path, uint field, ulong value, int mboard);
    // uhd_usrp_read_register(uhd_usrp_handle h, string path, uint field, int mboard, out ulong value_out);

    // uhd_usrp_clock_find(string args, out uhd_string_vector_t devices_out);
    // uhd_usrp_clock_make(out uhd_usrp_clock_handle h, string args);
    // uhd_usrp_clock_free(ref uhd_usrp_clock_handle h);
    // uhd_usrp_clock_last_error(uhd_usrp_clock_handle h, [Out] char[] error_out, nint strbuffer_len);
    // uhd_usrp_clock_get_pp_string(uhd_usrp_clock_handle h, [Out] char[] pp_string_out, nint strbuffer_len);
    // uhd_usrp_clock_get_num_boards(uhd_usrp_clock_handle h, out nuint num_boards_out);
    // uhd_usrp_clock_get_time(uhd_usrp_clock_handle h, nuint board, out uint clock_time_out);
    // uhd_usrp_clock_get_sensor(uhd_usrp_clock_handle h, string name, nuint board, out uhd_sensor_value_handle sensor_value_out);
    // uhd_usrp_clock_get_sensor_names(uhd_usrp_clock_handle h, nuint board, ref uhd_string_vector_handle sensor_names_out);

    public static string[] Find(string args = "")
    {
        using var strings = new StringVector();

        Raise(uhd_usrp_find(args, ref strings.Handle));

        var result = new List<string>();
        for (int i = 0; i < strings.Length; i++)
            result.Add(strings[i]);

        return [.. result];
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            Raise(uhd_usrp_free(ref handle));

            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~USRP()
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