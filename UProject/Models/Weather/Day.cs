using Newtonsoft.Json;

namespace UProject.Models.Weather
{
    public class Day
    {
        [JsonProperty("avgtemp_c")]
        public float AverageTemperatureCelsius { get; set; }

        [JsonProperty("maxwind_kph")]
        public float MaxWindSpeedKph { get; set; }

        [JsonProperty("daily_will_it_rain")]
        public int DailyWillItRain { get; set; }

        [JsonProperty("daily_will_it_snow")]
        public int DailyWillItSnow { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; }
    }
}
