
using Microsoft.VisualBasic;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using UProject.Models;

namespace UProject.Services.NotificationServices
{
    public class Sender : BackgroundService
    {
        private readonly ConcurrentQueue<BotMessage> _notificationQueue;

        private readonly ITelegramBotClient _telegramBotClient;

        public Sender(ConcurrentQueue<BotMessage> notificationQueue, ITelegramBotClient telegramBotClient)
        {
            _notificationQueue = notificationQueue;
            _telegramBotClient = telegramBotClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TimeSpan delay = TimeSpan.FromSeconds(0.05);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_notificationQueue.TryDequeue(out var message))
                    {
                        await _telegramBotClient.SendTextMessageAsync(message.Id, message.Text, replyMarkup: message.ReplyMarkup, cancellationToken: stoppingToken);
                    }
                }
                catch (ApiRequestException ex)
                {
                    if (ex.InnerException is ApiRequestException reqex && reqex.Parameters?.RetryAfter != null)
                    {
                        await Task.Delay(TimeSpan.FromMinutes((double)reqex.Parameters.RetryAfter), stoppingToken);
                    }
                }
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
