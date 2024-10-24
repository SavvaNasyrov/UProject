
using System.Collections.Concurrent;
using Telegram.Bot;
using UProject.Models;

namespace UProject.Services.NotificationServices
{
    public class OneDayNotifyer : BackgroundService
    {
        private readonly ILogger<OneMinuteNotifyer> _logger;

        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentQueue<BotMessage> _queue;

        public OneDayNotifyer(ILogger<OneMinuteNotifyer> logger, IServiceProvider scopeFactory, ConcurrentQueue<BotMessage> queue)
        {
            _logger = logger;
            _serviceProvider = scopeFactory;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var weather = scope.ServiceProvider.GetRequiredService<WeatherForecast>();

                foreach (var user in db.Users.Where(x => x.NotificationInterval == Interval.OneDay))
                {
                    _queue.Enqueue(new BotMessage
                    {
                        Id = user.Id,
                        Text = weather.GetForCity(user.City)
                    });

                    if (stoppingToken.IsCancellationRequested)
                        break;
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
