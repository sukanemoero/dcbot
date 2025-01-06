using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dcbot.Exceptions.Database;
using MySql.Data.MySqlClient;

namespace dcbot.Database;

public class Mysql(string host, string user, string password, string database)
{
    private readonly string _cs = $"server={host};user={user};password={password};database={database}";
    private MySqlConnection Connection { get; set; }

    public Mysql Connect()
    {
        Connection?.Dispose();
        Console.WriteLine("Connecting to database... ");
        Connection = new MySqlConnection(_cs);
        Connection.Open();
        Console.WriteLine(Connection.Ping()
            ? $"Database Connected: {Connection.ServerVersion}"
            : "Database Not Connected");
        if (Connection.Ping()) return this;
        throw new DatabaseNotConnected();


    }

    public class TableValue(string table, string value)
    {
        public override string ToString()
        {
            return $"`{Table}` . `{Value}`";
        }

        public string Table { get; } = table;
        public string Value { get; } = value;
    }

    public List<Dictionary<string, object>> FindWithJoinTable(
        List<string> tables,
        Dictionary<TableValue, object> filters,
        Dictionary<TableValue, TableValue> targetFiltersKeyToKey,
        List<TableValue> projections = null,
        int limit = 0,
        TableValue orderBy = null,
        bool count = false,
        TableValue sumColumn = null
    )
    {
        if (tables is not { Count: > 0 }) return [];
        var results = new List<Dictionary<string, object>>();
        var projectionString = "";
        if (count) projectionString = $" COUNT(*) as {tables.First()}";
        else if (sumColumn is not null) projectionString = $" SUM({sumColumn}) as {tables.First()} ";
        else if (projections is not { Count: > 0 }) projectionString = " * ";
        else
            foreach (var value in projections)
            {
                if (projectionString != "") projectionString += " , ";
                projectionString += $" {value} ";
            }

        var queryBuilder = new StringBuilder($"SELECT {projectionString} FROM `{tables.First()}`");

        if (tables is { Count: > 1 })
            foreach (var table in tables.Where(table => table != tables.First()))
            {
                queryBuilder.Append($" INNER JOIN `{table}` ");
                var keyValuePair = targetFiltersKeyToKey.FirstOrDefault(keyValuePair => keyValuePair.Key.Table == table,
                    new KeyValuePair<TableValue, TableValue>(null, null));
                if (keyValuePair.Key != null) queryBuilder.Append($" ON {keyValuePair.Key} = {keyValuePair.Value} ");
            }

        if (filters is { Count: > 0 })
        {
            queryBuilder.Append(" WHERE ");
            var filterClauses = filters.Select(filter =>
                ulong.TryParse(filter.Value.ToString(), out var v)
                    ? $"{filter.Key} = {v}"
                    : $"{filter.Key} = @{filter.Key.Table}{filter.Key.Value}").ToList();
            queryBuilder.Append(string.Join(" AND ", filterClauses));
        }

        if (limit > 0) queryBuilder.Append(" LIMIT @limit");
        if (orderBy is not null) queryBuilder.Append($" ORDER BY {orderBy}");
        using var command = new MySqlCommand(queryBuilder.ToString(), Connection);
        if (filters != null)
            foreach (var filter in filters)
                command.Parameters.AddWithValue($"@{filter.Key.Table}{filter.Key.Value}", filter.Value);

        command.Parameters.AddWithValue("@limit", limit);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var row = new Dictionary<string, object>();
            for (var i = 0; i < reader.FieldCount; i++) row[reader.GetName(i)] = reader.GetValue(i);
            results.Add(row);
        }

        return results;
    }

    public void Update(string table, Dictionary<string, object> values, Dictionary<string, object> filters)
    {
        if (values == null || values.Count == 0 || filters == null || filters.Count == 0) return;

        var queryBuilder = new StringBuilder($"UPDATE `{table}` SET ");
        var setClauses = values.Keys.Select(key => $"`{key}` = @{key}").ToList();

        queryBuilder.Append(string.Join(", ", setClauses));

        var filterClauses = filters.Select(filter => $"`{filter.Key}` = @{filter.Key}_filter").ToList();

        queryBuilder.Append(" WHERE " + string.Join(" AND ", filterClauses));

        using var command = new MySqlCommand(queryBuilder.ToString(), Connection);
        foreach (var kvp in values) command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
        foreach (var filter in filters) command.Parameters.AddWithValue($"@{filter.Key}_filter", filter.Value);

        command.ExecuteNonQuery();
    }

    public void Insert(string table, Dictionary<string, object> values, string[] ignore = null,
        string[] ignoreButAdjust = null)
    {
        if (values == null || values.Count == 0) return;
        var queryBuilder = new StringBuilder($"INSERT INTO `{table}` (");
        var valueBuilder = new StringBuilder(" VALUES (");

        List<string> parameters = [];

        foreach (var key in values.Keys)
        {
            parameters.Add($"@{key}");
            queryBuilder.Append($"`{key}`, ");
            valueBuilder.Append($"@{key}, ");
        }

        queryBuilder.Length -= 2;
        valueBuilder.Length -= 2;

        queryBuilder.Append(')');
        valueBuilder.Append(')');

        queryBuilder.Append(valueBuilder);

        if (ignore != null || ignoreButAdjust != null)
        {
            queryBuilder.Append($"  ON DUPLICATE KEY UPDATE  ");
            List<string> igs = [];

            igs.AddRange((ignore ?? []).Select(ign => $"{ign} = {ign}").ToList());
            igs.AddRange((ignoreButAdjust ?? []).Select(ign => $"{ign} = {ign} + VALUES ( {ign} )").ToList());

            queryBuilder.Append(string.Join(" , ", igs));
        }

        using var command = new MySqlCommand(queryBuilder.ToString(), Connection);
        foreach (var kvp in values) command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);

        command.ExecuteNonQuery();
    }

    public List<Dictionary<string, object>> Find(string table, Dictionary<string, object> filters, int limit = 10,
        bool filterByOr = false, bool rand = false, bool count = false)
    {
        var results = new List<Dictionary<string, object>>();
        var queryBuilder = count
            ? new StringBuilder($"SELECT COUNT(*) as `{table}` FROM `{table}`")
            : new StringBuilder($"SELECT * FROM `{table}`");

        if (filters is { Count: > 0 })
        {
            queryBuilder.Append(" WHERE ");
            var filterClauses = filters.Select(filter =>
                ulong.TryParse(filter.Value.ToString(), out var v)
                    ? $"{filter.Key} = {v}"
                    : $"{filter.Key} = @{filter.Key}").ToList();
            queryBuilder.Append(string.Join(filterByOr ? " OR " : " AND ", filterClauses));
        }

        if (rand) queryBuilder.Append(" ORDER BY RAND()");

        if (limit > 0) queryBuilder.Append(" LIMIT @limit");

        using var command = new MySqlCommand(queryBuilder.ToString(), Connection);
        if (filters != null)
            foreach (var filter in filters)
                command.Parameters.AddWithValue($"@{filter.Key}", filter.Value);

        command.Parameters.AddWithValue("@limit", limit);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var row = new Dictionary<string, object>();
            for (var i = 0; i < reader.FieldCount; i++) row[reader.GetName(i)] = reader.GetValue(i);
            results.Add(row);
        }

        return results;
    }


    public void Adjust(string table, string column, decimal amount, Dictionary<string, object> filters)
    {
        if (filters == null || filters.Count == 0 || table == null || column == null) return;

        var queryBuilder = new StringBuilder($"UPDATE `{table}` SET `{column}` = `{column}` + @amount");

        if (filters is { Count: > 0 })
        {
            queryBuilder.Append(" WHERE ");
            var filterClauses = filters.Select(filter => $"{filter.Key} = @{filter.Key}").ToList();
            queryBuilder.Append(string.Join(" AND ", filterClauses));
        }


        using var command = new MySqlCommand(queryBuilder.ToString(), Connection);
        command.Parameters.AddWithValue("@amount", amount);

        foreach (var filter in filters) command.Parameters.AddWithValue($"@{filter.Key}", filter.Value);

        command.ExecuteNonQuery();
    }

    public uint TypeCount(TableValue tableValue)
    {
        using var command = new MySqlCommand($"SELECT DISTINCT `{tableValue.Value}` FROM `{tableValue.Table}`", Connection);
        using var reader = command.ExecuteReader();
        var c = 0U;
        while (reader.Read()) c++;
        return c;
    }
    public bool IsConnected()
    {
        return Connection.Ping();
    }

    public void Disconnect()
    {
        Connection.Close();
    }
}