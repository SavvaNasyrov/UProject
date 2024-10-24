using Newtonsoft.Json;

namespace UProject.Models.Weather
{
    public class Forecast
    {
        [JsonProperty("forecastday")]
        public ForecastDay[] ForecastDays { get; set; }
    }
}
