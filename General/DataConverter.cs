using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using dcbot.Exceptions;

namespace dcbot.General;

public class DataConverter
{
    public static string StringConvert(string table, Dictionary<string, object> data)
    {
        return data.TryGetValue(table, out var stringObject)
            ? stringObject.ToString()
            : throw new GetWrongData($"Unable to convert table to string: table: {table}");
    }

    public static T NumberConvert<T>(string table, Dictionary<string, object> data) where T : INumber<T>
    {
        return T.TryParse(StringConvert(table, data), null, out var outputNumber)
            ? outputNumber
            : T.Zero;
    }

    public static bool BooleanConvert(string table, Dictionary<string, object> data)
    {
        return bool.TryParse(StringConvert(table, data), out var outputBoolean) && outputBoolean;
    }
}