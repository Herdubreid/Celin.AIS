using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Celin.AIS
{
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
            using var token = JsonDocument.Parse(json);
            token.RootElement.WriteTo(writer);
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
            using var token = JsonDocument.Parse(json);
            token.RootElement.WriteTo(writer);
        }
    }
}
