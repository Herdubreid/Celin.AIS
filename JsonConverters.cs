using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Celin.AIS
{
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
            return default(DateTimeOffset);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
    public class DateJsonConverter : JsonConverter<DateTime>
    {
        static readonly Regex PAT = new Regex(@"^(\d{4})(\d{2})(\d{2})$");
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<string>(ref reader, options);
            if (!string.IsNullOrEmpty(json))
            {
                var m = PAT.Match(json);
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
            return default(DateTime);
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
                : (Action)JsonSerializer.Deserialize<GridAction>(json.ToString(), options);
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
                : (Grid)JsonSerializer.Deserialize<GridUpdate>(json.ToString(), options);
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
