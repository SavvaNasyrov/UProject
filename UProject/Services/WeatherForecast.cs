namespace UProject.Services
{
    public class WeatherForecast
    {
        private readonly HttpClient _httpClient;

        public WeatherForecast(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string GetForCity(string city)
        {
            return city + " прогноз";
        }
    }
}
