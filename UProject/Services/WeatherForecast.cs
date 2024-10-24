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

        public string Daily(string city)
        {
            return "daily for city " + city;
        }

        public string Weekly(string city)
        {
            return "weekly for city " + city;
        }
    }
}
