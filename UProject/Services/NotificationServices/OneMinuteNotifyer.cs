
using Telegram.Bot;

namespace UProject.Services.NotificationServices
{
    public class OneMinuteNotifyer : BackgroundService
    {
        private readonly ILogger<OneMinuteNotifyer> _logger;

        private readonly IServiceProvider _serviceProvider;

        public OneMinuteNotifyer(ILogger<OneMinuteNotifyer> logger, IServiceProvider scopeFactory)
        {
            _logger = logger;
            _serviceProvider = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
                var weather = scope.ServiceProvider.GetRequiredService<WeatherForecast>();

                foreach (var user in db.Users.Where(x => x.NotificationInterval == Models.Interval.OneMinute))
                {
                    await botClient.SendTextMessageAsync(user.Id, weather.GetForCity(user.City), cancellationToken: stoppingToken);
                    await Task.Delay(TimeSpan.FromMilliseconds(30), stoppingToken);

                    if (stoppingToken.IsCancellationRequested)
                        break;
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
