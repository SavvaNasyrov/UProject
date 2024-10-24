using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Text;
using UProject.Models.Weather;

namespace UProject.Services
{
    public class WeatherForecast
    {
        private readonly HttpClient _httpClient;

        private readonly string _token;

        private readonly IMemoryCache _cache;

        public WeatherForecast(HttpClient httpClient, IConfiguration config, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://api.weatherapi.com/");
            _token = config["WeatherToken"] ?? throw new NullReferenceException("Token was null");
            _cache = cache;
        }

        public async Task<string> DailyAsync(string city)
        {
            if (!_cache.TryGetValue(city + "daily", out CurrentWeather? weather) || weather == null)
            {
                var resp = await _httpClient.GetAsync($"v1/current.json?key={_token}&q={city}&lang=ru");

                if (!resp.IsSuccessStatusCode)
                    return "Что-то пошло не так";

                var jsonContent = await resp.Content.ReadAsStringAsync();

                try
                {
                    weather = JToken.Parse(jsonContent).ToObject<CurrentWeather>() ?? throw new Exception("null");
                }
                catch
                {
                    return "Что-то пошло не так";
                }

                _cache.Set(
                    city + "daily",
                    weather,
                    new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now + TimeSpan.FromHours(12) });
            }

            return "Погода на сегодня:\n" +
                 $"Температура: *{weather!.Current.TemperatureCelsius}°C*\n" +
                 $"Скорость ветра: *{weather.Current.WindSpeedKph} км\\ч*\n" +
                 $"*{weather.Current.Condition.Description}*\n";
        }

        public async Task<string> WeeklyAsync(string city)
        {
            if (!_cache.TryGetValue(city + "weekly", out WeeklyForecast? weather) || weather == null)
            {
                var resp = await _httpClient.GetAsync($"v1/forecast.json?key={_token}&q={city}&lang=ru&days=7");

                if (!resp.IsSuccessStatusCode)
                    return "Что-то пошло не так";

                var jsonContent = await resp.Content.ReadAsStringAsync();

                try
                {
                    weather = JToken.Parse(jsonContent).ToObject<WeeklyForecast>() ?? throw new Exception("null");
                }
                catch
                {
                    return "Что-то пошло не так";
                }

                _cache.Set(
                    city + "weekly",
                    weather,
                    new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now + TimeSpan.FromHours(12) });
            }

            var sb = new StringBuilder("Погода на неделю: \n");
            
            foreach (var day in weather.Forecast.ForecastDays)
            {
                sb.Append($"{day.Date}:\n" +
                    $"Температура: *{day.Day.AverageTemperatureCelsius}°C*\n" +
                 $"Скорость ветра: *{day.Day.MaxWindSpeedKph} км\\ч*\n" +
                 $"*{day.Day.Condition.Description}*\n\n");
            }

            return sb.ToString();
        }

        public async Task<string[]> GetCityAsync(string city)
        {
            var resp = await _httpClient.GetAsync($"v1/search.json?key={_token}&q={city}");

            if (!resp.IsSuccessStatusCode)
                return Array.Empty<string>();

            var jsonContent = await resp.Content.ReadAsStringAsync();

            

            try
            {
                return JArray.Parse(jsonContent)
                    .ToObject<SearchResult[]>()?
                    .Select(x => x.Name)
                    .ToArray() ?? throw new Exception("null");
            }
            catch
            {
                return Array.Empty<string>();
            }
        }
    }
}
