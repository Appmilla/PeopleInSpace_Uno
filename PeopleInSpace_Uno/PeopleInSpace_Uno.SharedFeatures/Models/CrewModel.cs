using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PeopleInSpace_Uno.SharedFeatures.Models
{    
    public partial class CrewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("agency")]
        public Agency Agency { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; }

        [JsonProperty("wikipedia")]
        public Uri Wikipedia { get; set; }

        [JsonProperty("launches")]
        public Launch[] Launches { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public string AgencyString => Agency.ToString();
    }

    public enum Agency { Esa, Jaxa, Nasa, SpaceX };

    public enum Launch { The5Eb87D46Ffd86E000604B388, The5Eb87D4Dffd86E000604B38E, The5Fe3Af58B3467846B324215F };

    public enum Status { Active };

    public partial class CrewModel
    {
        public static CrewModel[] FromJson(string json) => JsonConvert.DeserializeObject<CrewModel[]>(json, PeopleInSpace_Uno.SharedFeatures.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CrewModel[] self) => JsonConvert.SerializeObject(self, PeopleInSpace_Uno.SharedFeatures.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                AgencyConverter.Singleton,
                LaunchConverter.Singleton,
                StatusConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class AgencyConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Agency) || t == typeof(Agency?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "ESA":
                    return Agency.Esa;
                case "JAXA":
                    return Agency.Jaxa;
                case "NASA":
                    return Agency.Nasa;
                case "SpaceX":
                    return Agency.SpaceX;
            }
            throw new Exception("Cannot unmarshal type Agency");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Agency)untypedValue;
            switch (value)
            {
                case Agency.Esa:
                    serializer.Serialize(writer, "ESA");
                    return;
                case Agency.Jaxa:
                    serializer.Serialize(writer, "JAXA");
                    return;
                case Agency.Nasa:
                    serializer.Serialize(writer, "NASA");
                    return;
                case Agency.SpaceX:
                    serializer.Serialize(writer, "SpaceX");
                    return;
            }
            throw new Exception("Cannot marshal type Agency");
        }

        public static readonly AgencyConverter Singleton = new AgencyConverter();
    }

    internal class LaunchConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Launch) || t == typeof(Launch?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "5eb87d46ffd86e000604b388":
                    return Launch.The5Eb87D46Ffd86E000604B388;
                case "5eb87d4dffd86e000604b38e":
                    return Launch.The5Eb87D4Dffd86E000604B38E;
                case "5fe3af58b3467846b324215f":
                    return Launch.The5Fe3Af58B3467846B324215F;
            }
            throw new Exception("Cannot unmarshal type Launch");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Launch)untypedValue;
            switch (value)
            {
                case Launch.The5Eb87D46Ffd86E000604B388:
                    serializer.Serialize(writer, "5eb87d46ffd86e000604b388");
                    return;
                case Launch.The5Eb87D4Dffd86E000604B38E:
                    serializer.Serialize(writer, "5eb87d4dffd86e000604b38e");
                    return;
                case Launch.The5Fe3Af58B3467846B324215F:
                    serializer.Serialize(writer, "5fe3af58b3467846b324215f");
                    return;
            }
            throw new Exception("Cannot marshal type Launch");
        }

        public static readonly LaunchConverter Singleton = new LaunchConverter();
    }

    internal class StatusConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Status) || t == typeof(Status?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "active")
            {
                return Status.Active;
            }
            throw new Exception("Cannot unmarshal type Status");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Status)untypedValue;
            if (value == Status.Active)
            {
                serializer.Serialize(writer, "active");
                return;
            }
            throw new Exception("Cannot marshal type Status");
        }

        public static readonly StatusConverter Singleton = new StatusConverter();
    }
}
