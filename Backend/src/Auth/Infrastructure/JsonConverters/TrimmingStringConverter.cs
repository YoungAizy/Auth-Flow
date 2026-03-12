using System.Text.Json;
using System.Text.Json.Serialization;

namespace Auth.Infrastructure.JsonConverters
{
    public class TrimmingStringConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Trim the string as it's being read from the JSON body
            return reader.GetString()?.Trim();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }

}
