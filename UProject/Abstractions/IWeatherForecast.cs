namespace UProject.Abstractions
{
    public interface IWeatherForecast
    {
        Task<string> DailyAsync(string city);
        Task<string> WeeklyAsync(string city);
        Task<string[]> GetCityAsync(string city);
    }
}
