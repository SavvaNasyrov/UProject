using Newtonsoft.Json;

namespace UProject.Models.Weather
{
    public class CurrentWeather
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("current")]
        public Current Current { get; set; }
    }
}
