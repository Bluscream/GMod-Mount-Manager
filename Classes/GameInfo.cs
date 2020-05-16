using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace GModMountManager.Classes
{
    public partial class GameInfo2
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("game", NullValueHandling = NullValueHandling.Ignore)]
        public string Game { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("title2", NullValueHandling = NullValueHandling.Ignore)]
        public string Title2 { get; set; }

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string Icon { get; set; }

        [JsonProperty("gamelogo", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Gamelogo { get; set; }

        [JsonProperty("developer", NullValueHandling = NullValueHandling.Ignore)]
        public string Developer { get; set; }

        [JsonProperty("developer_url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri DeveloperUrl { get; set; }

        [JsonProperty("homepage", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Homepage { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("SupportsDX8", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? SupportsDx8 { get; set; }

        [JsonProperty("SupportsXbox360", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? SupportsXbox360 { get; set; }

        [JsonProperty("nomodels", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Nomodels { get; set; }

        [JsonProperty("supportsvr", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? SupportsVR { get; set; }

        [JsonProperty("nocrosshair", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Nocrosshair { get; set; }

        [JsonProperty("GameData", NullValueHandling = NullValueHandling.Ignore)]
        public string GameData { get; set; }

        [JsonProperty("InstancePath", NullValueHandling = NullValueHandling.Ignore)]
        public string InstancePath { get; set; }

        [JsonProperty("FileSystem", NullValueHandling = NullValueHandling.Ignore)]
        public FileSystem FileSystem { get; set; }
    }

    public partial class FileSystem
    {
        [JsonProperty("SteamAppId", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? SteamAppId { get; set; }

        [JsonProperty("ToolsAppId", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? ToolsAppId { get; set; }

        [JsonProperty("SearchPaths", NullValueHandling = NullValueHandling.Ignore)]
        public SearchPaths SearchPaths { get; set; }
    }

    public partial class SearchPaths
    {
        [JsonProperty("game+mod", NullValueHandling = NullValueHandling.Ignore)]
        public string GameMod { get; set; }

        [JsonProperty("game", NullValueHandling = NullValueHandling.Ignore)]
        public string Game { get; set; }

        [JsonProperty("platform", NullValueHandling = NullValueHandling.Ignore)]
        public string Platform { get; set; }

        [JsonProperty("game+mod+mod_write+default_write_path", NullValueHandling = NullValueHandling.Ignore)]
        public string GameModModWriteDefaultWritePath { get; set; }

        [JsonProperty("gamebin", NullValueHandling = NullValueHandling.Ignore)]
        public string Gamebin { get; set; }

        [JsonProperty("game+game_write", NullValueHandling = NullValueHandling.Ignore)]
        public string GameGameWrite { get; set; }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}