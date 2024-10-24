using Newtonsoft.Json;

namespace UProject.Models.Weather
{
    public class Location
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("lat")]
        public float Latitude { get; set; }

        [JsonProperty("lon")]
        public float Longitude { get; set; }

        [JsonProperty("tz_id")]
        public string TimeZoneId { get; set; }

        [JsonProperty("localtime_epoch")]
        public int LocalTimeEpoch { get; set; }

        [JsonProperty("localtime")]
        public string LocalTime { get; set; }
    }
}
