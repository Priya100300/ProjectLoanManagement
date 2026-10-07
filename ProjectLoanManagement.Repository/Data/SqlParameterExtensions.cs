using System.Data;
using Microsoft.Data.SqlClient;

namespace ProjectLoanManagement.Repository.Data;

/// <summary>
/// Typed parameter helpers. Explicit SqlDbType + size avoids implicit conversions,
/// and null / blank values are sent as DBNull.
/// </summary>
public static class SqlParameterExtensions
{
    public static void AddInt(this SqlParameterCollection parameters, string name, int? value)
    {
        parameters.Add(name, SqlDbType.Int).Value = ToDb(value);
    }

    public static void AddBigInt(this SqlParameterCollection parameters, string name, long? value)
    {
        parameters.Add(name, SqlDbType.BigInt).Value = ToDb(value);
    }

    public static void AddBit(this SqlParameterCollection parameters, string name, bool? value)
    {
        parameters.Add(name, SqlDbType.Bit).Value = ToDb(value);
    }

    public static void AddNVarChar(this SqlParameterCollection parameters, string name, string value, int size)
    {
        parameters.Add(name, SqlDbType.NVarChar, size).Value = ToDb(value);
    }

    public static void AddVarChar(this SqlParameterCollection parameters, string name, string value, int size)
    {
        parameters.Add(name, SqlDbType.VarChar, size).Value = ToDb(value);
    }

    public static void AddChar(this SqlParameterCollection parameters, string name, string value, int size)
    {
        parameters.Add(name, SqlDbType.Char, size).Value = ToDb(value);
    }

    public static void AddDecimal(this SqlParameterCollection parameters, string name, decimal? value, byte precision = 18, byte scale = 2)
    {
        SqlParameter parameter = parameters.Add(name, SqlDbType.Decimal);
        parameter.Precision = precision;
        parameter.Scale = scale;
        parameter.Value = ToDb(value);
    }

    public static void AddDate(this SqlParameterCollection parameters, string name, DateTime? value)
    {
        parameters.Add(name, SqlDbType.Date).Value = value.HasValue ? value.Value.Date : DBNull.Value;
    }

    public static void AddDateTime2(this SqlParameterCollection parameters, string name, DateTime? value)
    {
        parameters.Add(name, SqlDbType.DateTime2).Value = ToDb(value);
    }

    public static void AddVarBinary(this SqlParameterCollection parameters, string name, byte[] value, int size)
    {
        parameters.Add(name, SqlDbType.VarBinary, size).Value = ToDb(value);
    }

    private static object ToDb(object value)
    {
        if (value == null)
        {
            return DBNull.Value;
        }

        if (value is string text && string.IsNullOrWhiteSpace(text))
        {
            return DBNull.Value;
        }

        return value;
    }
}
