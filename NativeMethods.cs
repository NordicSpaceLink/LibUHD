//
// Copyright 2015-2016 Ettus Research LLC
// Copyright 2018 Ettus Research, a National Instruments Company
//
// SPDX-License-Identifier: GPL-3.0-or-later
//

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NordicSpaceLink.LibUHD;

internal static class NativeMethods
{
    const string DllName = "uhd";

    public static bool Loaded { get; }

    public const string UHD_USRP_ALL_LOS = "all";
    public const float uhd_default_thread_priority = 0.5f;

    static NativeMethods()
    {
        NativeLibrary.SetDllImportResolver(typeof(NativeMethods).Assembly, Resolve);

        try
        {
            var buffer = new char[1024];
            uhd_get_last_error(buffer, buffer.Length);
            Loaded = true;
        }
        catch (DllNotFoundException)
        {
        }
    }

    private static nint Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName == DllName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (NativeLibrary.TryLoad($"lib{libraryName}.so", out var handle2))
                    return handle2;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (NativeLibrary.TryLoad($"lib{libraryName}.dylib", out var handle))
                    return handle;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (NativeLibrary.TryLoad($"{libraryName}.dll", out var handle))
                    return handle;
                if (NativeLibrary.TryLoad($"{libraryName}", out var handle2))
                    return handle2;
            }
        }

        return 0;
    }

    internal delegate uhd_error last_error([Out] char[] error_out, nint strbuffer_len);
    internal delegate uhd_error last_error<T>(T handle, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_get_last_error([Out] char[] error_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_get_abi_string([Out] char[] abi_string_out, nint buffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_get_version_string([Out] char[] version_out, nint buffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_make(out uhd_rx_metadata_handle handle);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_free(ref uhd_rx_metadata_handle handle);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_has_time_spec(uhd_rx_metadata_handle h, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_time_spec(uhd_rx_metadata_handle h, out long full_secs_out, out double frac_secs_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_more_fragments(uhd_rx_metadata_handle h, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_fragment_offset(uhd_rx_metadata_handle h, out nuint fragment_offset_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_start_of_burst(uhd_rx_metadata_handle h, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_end_of_burst(uhd_rx_metadata_handle h, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_out_of_sequence(uhd_rx_metadata_handle h, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_to_pp_string(uhd_rx_metadata_handle h, [Out] char[] pp_string_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_error_code(uhd_rx_metadata_handle h, out RxMetadataErrorCode error_code_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_strerror(uhd_rx_metadata_handle h, [Out] char[] strerror_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_metadata_last_error(uhd_rx_metadata_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_metadata_make(out uhd_tx_metadata_handle handle, bool has_time_spec, long full_secs, double frac_secs, bool start_of_burst, bool end_of_burst);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_metadata_free(ref uhd_tx_metadata_handle handle);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_metadata_has_time_spec(uhd_tx_metadata_handle h, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_metadata_time_spec(uhd_tx_metadata_handle h, out long full_secs_out, out double frac_secs_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_metadata_start_of_burst(uhd_tx_metadata_handle h, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_metadata_end_of_burst(uhd_tx_metadata_handle h, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_metadata_last_error(uhd_tx_metadata_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_async_metadata_make(out uhd_async_metadata_handle handle);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_async_metadata_free(ref uhd_async_metadata_handle handle);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_async_metadata_channel(uhd_async_metadata_handle h, out nuint channel_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_async_metadata_has_time_spec(uhd_async_metadata_handle h, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_async_metadata_time_spec(uhd_async_metadata_handle h, out long full_secs_out, out double frac_secs_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_async_metadata_event_code(uhd_async_metadata_handle h, out AsyncMetadataEventCode event_code_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_async_metadata_user_payload(uhd_async_metadata_handle h, out UserPayload user_payload_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_async_metadata_last_error(uhd_async_metadata_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_range_to_pp_string(ref Range range, [Out] char[] pp_string_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_make(out uhd_meta_range_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_free(ref uhd_meta_range_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_start(uhd_meta_range_handle h, out double start_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_stop(uhd_meta_range_handle h, out double stop_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_step(uhd_meta_range_handle h, out double step_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_clip(uhd_meta_range_handle h, double value, bool clip_step, out double result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_size(uhd_meta_range_handle h, out nuint size_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_push_back(uhd_meta_range_handle h, ref Range range);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_at(uhd_meta_range_handle h, nuint num, out Range range_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_to_pp_string(uhd_meta_range_handle h, [Out] char[] pp_string_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_meta_range_last_error(uhd_meta_range_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_make(out uhd_sensor_value_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_make_frombool(out uhd_sensor_value_handle h, string name, bool value, string utrue, string ufalse);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_make_from_int(out uhd_sensor_value_handle h, string name, int value, string unit, string formatter);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_make_from_realnum(out uhd_sensor_value_handle h, string name, double value, string unit, string formatter);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_make_from_string(out uhd_sensor_value_handle h, string name, string value, string unit);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_free(ref uhd_sensor_value_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_to_bool(uhd_sensor_value_handle h, out bool value_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_to_int(uhd_sensor_value_handle h, out int value_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_to_realnum(uhd_sensor_value_handle h, out double value_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_name(uhd_sensor_value_handle h, [Out] char[] name_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_value(uhd_sensor_value_handle h, [Out] char[] value_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_unit(uhd_sensor_value_handle h, [Out] char[] unit_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_data_type(uhd_sensor_value_handle h, out uhd_sensor_value_data_type_t data_type_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_to_pp_string(uhd_sensor_value_handle h, [Out] char[] pp_string_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_sensor_value_last_error(uhd_sensor_value_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_string_vector_make(out uhd_string_vector_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_string_vector_free(ref uhd_string_vector_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_string_vector_push_back(ref uhd_string_vector_handle h, string value);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_string_vector_at(uhd_string_vector_handle h, nuint index, [Out] char[] value_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_string_vector_size(uhd_string_vector_handle h, out nuint size_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_string_vector_last_error(uhd_string_vector_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern void uhd_tune_result_to_pp_string(ref TuneResult tune_result, [Out] char[] pp_string_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_rx_info_free(ref uhd_usrp_rx_info_t rx_info);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_tx_info_free(ref uhd_usrp_tx_info_t tx_info);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_dboard_eeprom_make(out uhd_dboard_eeprom_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_dboard_eeprom_free(ref uhd_dboard_eeprom_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_dboard_eeprom_get_id(uhd_dboard_eeprom_handle h, [Out] char[] id_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_dboard_eeprom_set_id(uhd_dboard_eeprom_handle h, string id);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_dboard_eeprom_get_serial(uhd_dboard_eeprom_handle h, [Out] char[] serial_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_dboard_eeprom_set_serial(uhd_dboard_eeprom_handle h, string serial);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_dboard_eeprom_get_revision(uhd_dboard_eeprom_handle h, out int revision_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_dboard_eeprom_set_revision(uhd_dboard_eeprom_handle h, int revision);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_dboard_eeprom_last_error(uhd_dboard_eeprom_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_mboard_eeprom_make(out uhd_mboard_eeprom_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_mboard_eeprom_free(ref uhd_mboard_eeprom_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_mboard_eeprom_get_value(uhd_mboard_eeprom_handle h, string key, [Out] char[] value_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_mboard_eeprom_set_value(uhd_mboard_eeprom_handle h, string key, string value);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_mboard_eeprom_last_error(uhd_mboard_eeprom_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_subdev_spec_pair_free(ref uhd_subdev_spec_pair_t subdev_spec_pair);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_subdev_spec_pairs_equal(ref uhd_subdev_spec_pair_t first, ref uhd_subdev_spec_pair_t second, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_subdev_spec_make(out uhd_subdev_spec_handle h, string markup);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_subdev_spec_free(ref uhd_subdev_spec_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_subdev_spec_size(uhd_subdev_spec_handle h, out nuint size_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_subdev_spec_push_back(uhd_subdev_spec_handle h, string markup);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_subdev_spec_at(uhd_subdev_spec_handle h, nuint num, out uhd_subdev_spec_pair_t subdev_spec_pair_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_subdev_spec_to_pp_string(uhd_subdev_spec_handle h, [Out] char[] pp_string_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_subdev_spec_to_string(uhd_subdev_spec_handle h, [Out] char[] string_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_subdev_spec_last_error(uhd_subdev_spec_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_streamer_make(out uhd_rx_streamer_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_streamer_free(ref uhd_rx_streamer_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_streamer_num_channels(uhd_rx_streamer_handle h, out nuint num_channels_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_streamer_max_num_samps(uhd_rx_streamer_handle h, out nuint max_num_samps_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_streamer_recv(uhd_rx_streamer_handle h, ref IntPtr buffs, nuint samps_per_buff, ref uhd_rx_metadata_handle md, double timeout, bool one_packet, out nuint items_recvd);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_streamer_issue_stream_cmd(uhd_rx_streamer_handle h, ref StreamCommand stream_cmd);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_rx_streamer_last_error(uhd_rx_streamer_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_streamer_make(out uhd_tx_streamer_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_streamer_free(ref uhd_tx_streamer_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_streamer_num_channels(uhd_tx_streamer_handle h, out nuint num_channels_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_streamer_max_num_samps(uhd_tx_streamer_handle h, out nuint max_num_samps_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_streamer_send(uhd_tx_streamer_handle h, ref IntPtr buffs, nuint samps_per_buff, ref uhd_tx_metadata_handle md, double timeout, out nuint items_sent);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_streamer_recv_async_msg(uhd_tx_streamer_handle h, ref uhd_async_metadata_handle md, double timeout, ref bool valid);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_tx_streamer_last_error(uhd_tx_streamer_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_find(string args, ref uhd_string_vector_handle strings_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_make(out uhd_usrp_handle h, string args);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_free(ref uhd_usrp_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_last_error(uhd_usrp_handle h, [Out] char[] error_out, nint strbuffer_len);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_stream(uhd_usrp_handle h, ref UhdStreamArgs stream_args, uhd_rx_streamer_handle h_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_stream(uhd_usrp_handle h, ref UhdStreamArgs stream_args, uhd_tx_streamer_handle h_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_info(uhd_usrp_handle h, nuint chan, out uhd_usrp_rx_info_t info_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_info(uhd_usrp_handle h, nuint chan, out uhd_usrp_tx_info_t info_out);
    
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_master_clock_rate(uhd_usrp_handle h, double rate, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_master_clock_rate(uhd_usrp_handle h, nuint mboard, out double clock_rate_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_pp_string(uhd_usrp_handle h, [Out] char[] pp_string_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_mboard_name(uhd_usrp_handle h, nuint mboard, [Out] char[] mboard_name_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_time_now(uhd_usrp_handle h, nuint mboard, out long full_secs_out, out double frac_secs_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_time_last_pps(uhd_usrp_handle h, nuint mboard, out long full_secs_out, out double frac_secs_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_time_now(uhd_usrp_handle h, long full_secs, double frac_secs, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_time_next_pps(uhd_usrp_handle h, long full_secs, double frac_secs, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_time_unknown_pps(uhd_usrp_handle h, long full_secs, double frac_secs);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_time_synchronized(uhd_usrp_handle h, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_command_time(uhd_usrp_handle h, long full_secs, double frac_secs, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_clear_command_time(uhd_usrp_handle h, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_time_source(uhd_usrp_handle h, string time_source, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_time_source(uhd_usrp_handle h, nuint mboard, [Out] char[] time_source_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_time_sources(uhd_usrp_handle h, nuint mboard, ref uhd_string_vector_handle time_sources_out);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_clock_source(uhd_usrp_handle h, string clock_source, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_clock_source(uhd_usrp_handle h, nuint mboard, [Out] char[] clock_source_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_clock_sources(uhd_usrp_handle h, nuint mboard, ref uhd_string_vector_handle clock_sources_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_clock_source_out(uhd_usrp_handle h, bool enb, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_time_source_out(uhd_usrp_handle h, bool enb, nuint mboard);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_num_mboards(uhd_usrp_handle h, out nuint num_mboards_out);
    
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_mboard_sensor(uhd_usrp_handle h, string name, nuint mboard, ref uhd_sensor_value_handle sensor_value_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_mboard_sensor_names(uhd_usrp_handle h, nuint mboard, ref uhd_string_vector_handle mboard_sensor_names_out);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_user_register(uhd_usrp_handle h, byte addr, uint data, nuint mboard);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_mboard_eeprom(uhd_usrp_handle h, uhd_mboard_eeprom_handle mb_eeprom, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_mboard_eeprom(uhd_usrp_handle h, uhd_mboard_eeprom_handle mb_eeprom, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_dboard_eeprom(uhd_usrp_handle h, uhd_dboard_eeprom_handle db_eeprom, string unit, string slot, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_dboard_eeprom(uhd_usrp_handle h, uhd_dboard_eeprom_handle db_eeprom, string unit, string slot, nuint mboard);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_subdev_spec(uhd_usrp_handle h, uhd_subdev_spec_handle subdev_spec, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_subdev_spec(uhd_usrp_handle h, nuint mboard, uhd_subdev_spec_handle subdev_spec_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_num_channels(uhd_usrp_handle h, out nuint num_channels_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_subdev_name(uhd_usrp_handle h, nuint chan, [Out] char[] rx_subdev_name_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_rate(uhd_usrp_handle h, double rate, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_rate(uhd_usrp_handle h, nuint chan, out double rate_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_rates(uhd_usrp_handle h, nuint chan, uhd_meta_range_handle rates_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_freq(uhd_usrp_handle h, ref UhdTuneRequest tune_request, nuint chan, out TuneResult tune_result);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_freq(uhd_usrp_handle h, nuint chan, out double freq_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_freq_range(uhd_usrp_handle h, nuint chan, uhd_meta_range_handle freq_range_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_fe_rx_freq_range(uhd_usrp_handle h, nuint chan, uhd_meta_range_handle freq_range_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_lo_names(uhd_usrp_handle h, nuint chan, ref uhd_string_vector_handle rx_lo_names_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_lo_source(uhd_usrp_handle h, string src, string name, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_lo_source(uhd_usrp_handle h, string name, nuint chan, [Out] char[] rx_lo_source_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_lo_sources(uhd_usrp_handle h, string name, nuint chan, ref uhd_string_vector_handle rx_lo_sources_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_lo_export_enabled(uhd_usrp_handle h, bool enabled, string name, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_lo_export_enabled(uhd_usrp_handle h, string name, nuint chan, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_lo_freq(uhd_usrp_handle h, double freq, string name, nuint chan, out double coerced_freq_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_lo_freq(uhd_usrp_handle h, string name, nuint chan, out double rx_lo_freq_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_gain(uhd_usrp_handle h, double gain, nuint chan, string gain_name);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_normalized_rx_gain(uhd_usrp_handle h, double gain, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_agc(uhd_usrp_handle h, bool enable, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_gain(uhd_usrp_handle h, nuint chan, string gain_name, out double gain_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_normalized_rx_gain(uhd_usrp_handle h, nuint chan, out double gain_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_gain_range(uhd_usrp_handle h, string name, nuint chan, uhd_meta_range_handle gain_range_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_gain_names(uhd_usrp_handle h, nuint chan, ref uhd_string_vector_handle gain_names_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_antenna(uhd_usrp_handle h, string ant, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_antenna(uhd_usrp_handle h, nuint chan, [Out] char[] ant_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_antennas(uhd_usrp_handle h, nuint chan, ref uhd_string_vector_handle antennas_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_sensor_names(uhd_usrp_handle h, nuint chan, ref uhd_string_vector_handle sensor_names_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_bandwidth(uhd_usrp_handle h, double bandwidth, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_bandwidth(uhd_usrp_handle h, nuint chan, out double bandwidth_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_bandwidth_range(uhd_usrp_handle h, nuint chan, uhd_meta_range_handle bandwidth_range_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_rx_sensor(uhd_usrp_handle h, string name, nuint chan, ref uhd_sensor_value_handle sensor_value_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_dc_offset_enabled(uhd_usrp_handle h, bool enb, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_rx_iq_balance_enabled(uhd_usrp_handle h, bool enb, nuint chan);
    
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_tx_subdev_spec(uhd_usrp_handle h, uhd_subdev_spec_handle subdev_spec, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_subdev_spec(uhd_usrp_handle h, nuint mboard, uhd_subdev_spec_handle subdev_spec_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_num_channels(uhd_usrp_handle h, out nuint num_channels_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_subdev_name(uhd_usrp_handle h, nuint chan, [Out] char[] tx_subdev_name_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_tx_rate(uhd_usrp_handle h, double rate, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_rate(uhd_usrp_handle h, nuint chan, out double rate_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_rates(uhd_usrp_handle h, nuint chan, uhd_meta_range_handle rates_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_tx_freq(uhd_usrp_handle h, ref UhdTuneRequest tune_request, nuint chan, out TuneResult tune_result);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_freq(uhd_usrp_handle h, nuint chan, out double freq_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_freq_range(uhd_usrp_handle h, nuint chan, uhd_meta_range_handle freq_range_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_fe_tx_freq_range(uhd_usrp_handle h, nuint chan, uhd_meta_range_handle freq_range_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_lo_names(uhd_usrp_handle h, nuint chan, ref uhd_string_vector_handle tx_lo_names_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_tx_lo_source(uhd_usrp_handle h, string src, string name, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_lo_source(uhd_usrp_handle h, string name, nuint chan, [Out] char[] tx_lo_source_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_lo_sources(uhd_usrp_handle h, string name, nuint chan, ref uhd_string_vector_handle tx_lo_sources_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_tx_lo_export_enabled(uhd_usrp_handle h, bool enabled, string name, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_lo_export_enabled(uhd_usrp_handle h, string name, nuint chan, out bool result_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_tx_lo_freq(uhd_usrp_handle h, double freq, string name, nuint chan, out double coerced_freq_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_lo_freq(uhd_usrp_handle h, string name, nuint chan, out double tx_lo_freq_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_tx_gain(uhd_usrp_handle h, double gain, nuint chan, string gain_name);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_normalized_tx_gain(uhd_usrp_handle h, double gain, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_gain_range(uhd_usrp_handle h, string name, nuint chan, uhd_meta_range_handle gain_range_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_gain(uhd_usrp_handle h, nuint chan, string gain_name, out double gain_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_normalized_tx_gain(uhd_usrp_handle h, nuint chan, out double gain_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_gain_names(uhd_usrp_handle h, nuint chan, ref uhd_string_vector_handle gain_names_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_tx_antenna(uhd_usrp_handle h, string ant, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_antenna(uhd_usrp_handle h, nuint chan, [Out] char[] ant_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_antennas(uhd_usrp_handle h, nuint chan, ref uhd_string_vector_handle antennas_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_tx_bandwidth(uhd_usrp_handle h, double bandwidth, nuint chan);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_bandwidth(uhd_usrp_handle h, nuint chan, out double bandwidth_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_bandwidth_range(uhd_usrp_handle h, nuint chan, uhd_meta_range_handle bandwidth_range_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_sensor(uhd_usrp_handle h, string name, nuint chan, ref uhd_sensor_value_handle sensor_value_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_tx_sensor_names(uhd_usrp_handle h, nuint chan, ref uhd_string_vector_handle sensor_names_out);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_gpio_banks(uhd_usrp_handle h, nuint mboard, ref uhd_string_vector_handle gpio_banks_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_set_gpio_attr(uhd_usrp_handle h, string bank, string attr, uint value, uint mask, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_gpio_attr(uhd_usrp_handle h, string bank, string attr, nuint mboard, out uint attr_out);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_enumerate_registers(uhd_usrp_handle h, nuint mboard, ref uhd_string_vector_handle registers_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_get_register_info(uhd_usrp_handle h, string path, nuint mboard, out uhd_usrp_register_info_t register_info_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_write_register(uhd_usrp_handle h, string path, uint field, ulong value, nuint mboard);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_read_register(uhd_usrp_handle h, string path, uint field, nuint mboard, out ulong value_out);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_clock_find(string args, out uhd_string_vector_t devices_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_clock_make(out uhd_usrp_clock_handle h, string args);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_clock_free(ref uhd_usrp_clock_handle h);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_clock_last_error(uhd_usrp_clock_handle h, [Out] char[] error_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_clock_get_pp_string(uhd_usrp_clock_handle h, [Out] char[] pp_string_out, nint strbuffer_len);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_clock_get_num_boards(uhd_usrp_clock_handle h, out nuint num_boards_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_clock_get_time(uhd_usrp_clock_handle h, nuint board, out uint clock_time_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_clock_get_sensor(uhd_usrp_clock_handle h, string name, nuint board, ref uhd_sensor_value_handle sensor_value_out);
    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_usrp_clock_get_sensor_names(uhd_usrp_clock_handle h, nuint board, ref uhd_string_vector_handle sensor_names_out);

    [DllImport(DllName, CharSet = CharSet.Ansi)] public static extern uhd_error uhd_set_thread_priority(float priority, bool realtime);

    internal static void Raise(uhd_error uhd_error)
    {
        if (uhd_error != uhd_error.UHD_ERROR_NONE)
        {
            var error_str = uhd_error switch
            {
                uhd_error.UHD_ERROR_NONE => "none",
                uhd_error.UHD_ERROR_INVALID_DEVICE => "invalid_device",
                uhd_error.UHD_ERROR_INDEX => "index",
                uhd_error.UHD_ERROR_KEY => "key",
                uhd_error.UHD_ERROR_NOT_IMPLEMENTED => "not_implemented",
                uhd_error.UHD_ERROR_USB => "usb",
                uhd_error.UHD_ERROR_IO => "io",
                uhd_error.UHD_ERROR_OS => "os",
                uhd_error.UHD_ERROR_ASSERTION => "assertion",
                uhd_error.UHD_ERROR_LOOKUP => "lookup",
                uhd_error.UHD_ERROR_TYPE => "type",
                uhd_error.UHD_ERROR_VALUE => "value",
                uhd_error.UHD_ERROR_RUNTIME => "runtime",
                uhd_error.UHD_ERROR_ENVIRONMENT => "environment",
                uhd_error.UHD_ERROR_SYSTEM => "system",
                uhd_error.UHD_ERROR_EXCEPT => "except",
                uhd_error.UHD_ERROR_BOOSTEXCEPT => "boostexcept",
                uhd_error.UHD_ERROR_STDEXCEPT => "stdexcept",
                uhd_error.UHD_ERROR_UNKNOWN or _ => "unknown",
            };

            var buffer = new char[1024];
            uhd_get_last_error(buffer, buffer.Length);

            throw new Exception(new string(buffer));
        }
    }

    internal static void Raise<T>(uhd_error uhd_error, T handle, last_error<T> error_func)
    {
        if (uhd_error != uhd_error.UHD_ERROR_NONE)
        {
            var error_str = uhd_error switch
            {
                uhd_error.UHD_ERROR_NONE => "none",
                uhd_error.UHD_ERROR_INVALID_DEVICE => "invalid_device",
                uhd_error.UHD_ERROR_INDEX => "index",
                uhd_error.UHD_ERROR_KEY => "key",
                uhd_error.UHD_ERROR_NOT_IMPLEMENTED => "not_implemented",
                uhd_error.UHD_ERROR_USB => "usb",
                uhd_error.UHD_ERROR_IO => "io",
                uhd_error.UHD_ERROR_OS => "os",
                uhd_error.UHD_ERROR_ASSERTION => "assertion",
                uhd_error.UHD_ERROR_LOOKUP => "lookup",
                uhd_error.UHD_ERROR_TYPE => "type",
                uhd_error.UHD_ERROR_VALUE => "value",
                uhd_error.UHD_ERROR_RUNTIME => "runtime",
                uhd_error.UHD_ERROR_ENVIRONMENT => "environment",
                uhd_error.UHD_ERROR_SYSTEM => "system",
                uhd_error.UHD_ERROR_EXCEPT => "except",
                uhd_error.UHD_ERROR_BOOSTEXCEPT => "boostexcept",
                uhd_error.UHD_ERROR_STDEXCEPT => "stdexcept",
                uhd_error.UHD_ERROR_UNKNOWN or _ => "unknown",
            };

            var buffer = new char[1024];
            error_func(handle, buffer, buffer.Length);

            throw new Exception(new string(buffer));
        }
    }
}