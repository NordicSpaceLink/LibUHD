using System.Globalization;
using System.Runtime.CompilerServices;
using static NordicSpaceLink.LibUHD.NativeMethods;

namespace NordicSpaceLink.LibUHD;

public enum SensorValueType
{
    Unknown,

    Bool,
    Integer,
    Real,
    String,
}

public readonly record struct SensorValue(string Name, string Value, string Unit, SensorValueType ValueType)
{
    public readonly bool AsBool() => Value == "true";
    public readonly int AsInteger() => int.Parse(Value);
    public readonly double AsReal() => double.Parse(Value, CultureInfo.InvariantCulture);

    public override string ToString() => $"{Name}: {Value} {Unit} [{ValueType}]";
}

internal ref struct SensorValueHandle
{
    private uhd_sensor_value_handle handle;

    public ref uhd_sensor_value_handle Handle => ref Unsafe.AsRef(ref handle);

    public SensorValueHandle()
    {
        Raise(uhd_sensor_value_make(out handle));
    }

    public readonly SensorValue Value
    {
        get
        {
            using var buffer = new Pooled<char>(1024);

            Raise(uhd_sensor_value_name(handle, buffer.Buffer, buffer.Length), handle, uhd_sensor_value_last_error);
            var name = new string(buffer.Buffer);

            Raise(uhd_sensor_value_value(handle, buffer.Buffer, buffer.Length), handle, uhd_sensor_value_last_error);
            var value = new string(buffer.Buffer);

            Raise(uhd_sensor_value_unit(handle, buffer.Buffer, buffer.Length), handle, uhd_sensor_value_last_error);
            var unit = new string(buffer.Buffer);

            Raise(uhd_sensor_value_data_type(handle, out var dt));
            var type = dt switch
            {
                uhd_sensor_value_data_type_t.UHD_SENSOR_VALUE_BOOLEAN => SensorValueType.Bool,
                uhd_sensor_value_data_type_t.UHD_SENSOR_VALUE_INTEGER => SensorValueType.Integer,
                uhd_sensor_value_data_type_t.UHD_SENSOR_VALUE_REALNUM => SensorValueType.Real,
                uhd_sensor_value_data_type_t.UHD_SENSOR_VALUE_STRING => SensorValueType.String,
                _ => SensorValueType.Unknown,
            };

            return new(name, value, unit, type);
        }
    }

    public void Dispose()
    {
        Raise(uhd_sensor_value_free(ref handle));
    }

}