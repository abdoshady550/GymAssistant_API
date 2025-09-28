using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GymAssistant_API.Extensions
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] Formats =
            { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy/MM/dd", "yyyy/MM/ddTHH:mm:ss" };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (DateTime.TryParseExact(value, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }
            throw new JsonException($"Invalid date format: {value}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }
}
