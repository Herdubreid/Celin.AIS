using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Celin.AIS
{
    public static partial class JsonHelpers
    {
        [GeneratedRegex("^((?:19|20)[0-9][0-9])((?:0[1-9]|1[0-2]))((?:0[1-9]|[1-2][0-9]|3[0-1]))$")]
        public static partial Regex DATE();
        [GeneratedRegex(@"^(\d{4})(\d{2})(\d{2})$")]
        public static partial Regex YMD(); 

        [GeneratedRegex("z_(\\w+)_(\\d+)")]
        public static partial Regex FIELD();
        public record JsonField(string title, JsonElement value);
        public static object PropertyValue(JsonElement json)
        {
            switch (json.ValueKind)
            {
                case JsonValueKind.Number:
                    if (json.TryGetInt32(out var i)) return i;
                    else return json.GetDecimal();
                case JsonValueKind.String:
                    var m = DATE().Match(json.GetString() ?? string.Empty);
                    if (m.Success)
                    {
                        return DateTime.Parse($"{m.Groups[1].Value}-{m.Groups[2].Value}-{m.Groups[3].Value}").Date;
                    }
                    return json.GetString() as object;
                default:
                    return null;
            }
        }
    }
    public class FormFieldJsonConverter : JsonConverter<Dictionary<int, FormField>>
    {
        public override Dictionary<int, FormField> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);

            var res = json.EnumerateObject()
                .Select(e =>
                {
                    var m = JsonHelpers.FIELD().Match(e.Name);
                    if (m.Success)
                    {
                        var f = e.Value.Deserialize<JsonHelpers.JsonField>();
                        return new Tuple<int, FormField>(Convert.ToInt32(m.Groups[2].Value),
                            new FormField(m.Groups[1].Value, f.title, JsonHelpers.PropertyValue(f.value)));
                    }
                    return default;
                })
                .Where(e => e != default);
            return res.ToDictionary(e => e.Item1, e => e.Item2);
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<int, FormField> value, JsonSerializerOptions options)
            => throw new NotImplementedException();
    }
    public class ControlFieldJsonConverter : JsonConverter<Dictionary<int, ControlField>>
    {
        record type(string id, string title);
        public override Dictionary<int, ControlField> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<JsonElement> (ref reader, options);

            var ctrls = json.EnumerateObject()
                .Select(e =>
                {
                    ControlType t = e.Name.Substring(0, 3) switch
                    {
                        "mnu" => ControlType.Menu,
                        "row" => ControlType.Row,
                        "frm" => ControlType.Form,
                        _ => ControlType.Other
                    };
                    var c = JsonSerializer.Deserialize<type>(e.Value);

                    return new { t, c };
                });
            var res = ctrls.Where(e => e.t != ControlType.Other && int.TryParse(e.c.id, out _));
            return res.ToDictionary(e => int.Parse(e.c.id), e => new ControlField(e.t, e.c.title));
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<int, ControlField> value, JsonSerializerOptions options)
            => throw new NotImplementedException();
    }
    public class GridRowJsonConverter : JsonConverter<object[]>
    {
        public override object[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);

            return json.EnumerateObject().Select(c => JsonHelpers.PropertyValue(c.Value)).ToArray();
        }

        public override void Write(Utf8JsonWriter writer, object[] value, JsonSerializerOptions options)
            => throw new NotImplementedException();
    }
    public class UTimeJsonConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<string>(ref reader, options);
            if (DateTimeOffset.TryParse(json, null,
                System.Globalization.DateTimeStyles.AssumeUniversal, out var ts))
            {
                return ts;
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
    public class DateJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<string>(ref reader, options);
            if (!string.IsNullOrEmpty(json))
            {
                var m = JsonHelpers.YMD().Match(json);
                if (m.Success)
                {
                    try
                    {
                        return new DateTime(
                            Convert.ToInt32(m.Groups[1].Value),
                            Convert.ToInt32(m.Groups[2].Value),
                            Convert.ToInt32(m.Groups[3].Value));
                    }
                    catch { }
                }
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
    public class ActionJsonConverter : JsonConverter<Action>
    {
        public override Action Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
            return json.TryGetProperty("controlID", out JsonElement _)
                ? JsonSerializer.Deserialize<FormAction>(json.ToString(), options)
                : JsonSerializer.Deserialize<GridAction>(json.ToString(), options);
        }

        public override void Write(Utf8JsonWriter writer, Action value, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Serialize(value, value.GetType(), options);
            using (var token = JsonDocument.Parse(json))
            {
                token.RootElement.WriteTo(writer);
            }
        }
    }
    public class GridActionJsonConverter : JsonConverter<Grid>
    {
        public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
            return json.TryGetProperty("gridRowInsertEvents", out JsonElement _)
                ? JsonSerializer.Deserialize<GridInsert>(json.ToString(), options)
                : JsonSerializer.Deserialize<GridUpdate>(json.ToString(), options);
        }

        public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Serialize(value, value.GetType(), options);
            using (var token = JsonDocument.Parse(json))
            {
                token.RootElement.WriteTo(writer);
            }
        }
    }
}
