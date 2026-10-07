using Microsoft.Data.SqlClient;

namespace ProjectLoanManagement.Repository.Data;

public static class SqlReaderExtensions
{
    /// <summary>Reads a column by name; DBNull becomes default(T). Works for nullable types.</summary>
    public static T Field<T>(this SqlDataReader reader, string columnName)
    {
        object value = reader[columnName];
        if (value == null || value == DBNull.Value)
        {
            return default(T);
        }

        if (value is T typed)
        {
            return typed;
        }

        Type target = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T)Convert.ChangeType(value, target);
    }

    /// <summary>Reads every row of the current result set.</summary>
    public static List<T> ReadList<T>(this SqlDataReader reader, Func<SqlDataReader, T> map)
    {
        List<T> items = new List<T>();
        while (reader.Read())
        {
            items.Add(map(reader));
        }

        return items;
    }

    /// <summary>Reads the first row of the current result set, or default when there is none.</summary>
    public static T ReadSingle<T>(this SqlDataReader reader, Func<SqlDataReader, T> map)
    {
        return reader.Read() ? map(reader) : default(T);
    }

    /// <summary>Generic rows for reports: column name (camelCase) -> value.</summary>
    public static List<Dictionary<string, object>> ReadRows(this SqlDataReader reader)
    {
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        while (reader.Read())
        {
            Dictionary<string, object> row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                string key = name.Length > 0 ? char.ToLowerInvariant(name[0]) + name.Substring(1) : name;
                row[key] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }

            rows.Add(row);
        }

        return rows;
    }
}
