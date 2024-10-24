using Newtonsoft.Json;

namespace UProject.Models.Weather
{
    public class WeeklyForecast
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("current")]
        public Current Current { get; set; }

        [JsonProperty("forecast")]
        public Forecast Forecast { get; set; }
    }
}
