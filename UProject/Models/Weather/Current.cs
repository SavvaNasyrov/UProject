using Newtonsoft.Json;

namespace UProject.Models.Weather
{
    public class Current
    {
        [JsonProperty("temp_c")]
        public float TemperatureCelsius { get; set; }

        [JsonProperty("is_day")]
        public int IsDay { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; }

        [JsonProperty("wind_kph")]
        public float WindSpeedKph { get; set; }

        [JsonProperty("cloud")]
        public int CloudCoverage { get; set; }
    }
}
