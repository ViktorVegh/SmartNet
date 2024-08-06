using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Models;

namespace Models
{
    public class ContentJsonConverter : JsonConverter<Content>
    {
        public override Content Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                if (doc.RootElement.TryGetProperty("type", out JsonElement typeElement))
                {
                    string type = typeElement.GetString();
                    if (type != null)
                    {
                        switch (type)
                        {
                            case "photo":
                                return JsonSerializer.Deserialize<PhotoContent>(doc.RootElement.GetRawText(), options);
                            case "podcast":
                                return JsonSerializer.Deserialize<PodcastContent>(doc.RootElement.GetRawText(), options);
                            case "text":
                                return JsonSerializer.Deserialize<TextContent>(doc.RootElement.GetRawText(), options);
                            case "video":
                                return JsonSerializer.Deserialize<VideoContent>(doc.RootElement.GetRawText(), options);
                        }
                    }
                }
            }

            throw new JsonException("Unknown content type");
        }

        public override void Write(Utf8JsonWriter writer, Content value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            
            writer.WriteString("type", value.Type);
            
            foreach (var property in value.GetType().GetProperties())
            {
                if (property.Name == "LearningMaterial" || property.Name == "Id" || property.Name == "LearningMaterialId" || property.Name == "Type")
                    continue; 

                var propValue = property.GetValue(value);
                var propName = JsonNamingPolicy.CamelCase.ConvertName(property.Name);
                writer.WritePropertyName(propName);
                JsonSerializer.Serialize(writer, propValue, propValue?.GetType() ?? typeof(object), options);
            }

            writer.WriteEndObject();
        }
    }
}
