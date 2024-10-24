using Newtonsoft.Json;

namespace UProject.Models.Weather
{
    public class ForecastDay
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("day")]
        public Day Day { get; set; }
    }
}
