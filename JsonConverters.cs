using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Celin.AIS
{
    class ActionJsonConverter : JsonConverter<Action>
    {
        public override Action Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Action value, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Serialize(value, value.GetType(), options);
            using var token = JsonDocument.Parse(json);
            token.RootElement.WriteTo(writer);
        }
    }
    class GridActionJsonConverter : JsonConverter<Celin.AIS.Grid>
    {
        public override Celin.AIS.Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Celin.AIS.Grid value, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Serialize(value, value.GetType(), options);
            using var token = JsonDocument.Parse(json);
            token.RootElement.WriteTo(writer);
        }
    }
}
