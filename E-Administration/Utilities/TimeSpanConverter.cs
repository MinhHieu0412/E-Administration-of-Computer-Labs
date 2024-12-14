using System.Text.Json;
using System.Text.Json.Serialization;

namespace E_Administration.Utilities
{
    public class TimeSpanConverter: JsonConverter<TimeSpan>
    {
        // Chuyển đổi TimeSpan từ JSON sang TimeSpan
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            if (TimeSpan.TryParse(stringValue, out var result))
            {
                return result;
            }

            throw new JsonException($"Invalid TimeSpan value: {stringValue}");
        }

        // Chuyển đổi TimeSpan từ TimeSpan sang JSON
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(@"hh\:mm\:ss"));
        }
    }
}
