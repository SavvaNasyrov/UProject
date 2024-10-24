using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UProject.Models;
using UProject.Services;

namespace UProject.Controllers
{
    [Route("/")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;

        private readonly AppDbContext _db;

        private readonly ILogger<TelegramBotClient> _logger;

        private readonly ConcurrentQueue<BotMessage> _queue;

        private readonly WeatherForecast _weatherForecast;

        public TelegramController(ITelegramBotClient botClient, AppDbContext db, ILogger<TelegramBotClient> logger, ConcurrentQueue<BotMessage> queue, WeatherForecast weatherForecast)
        {
            _botClient = botClient;
            _db = db;
            _logger = logger;
            _queue = queue;
            _weatherForecast = weatherForecast;
        }

        [HttpGet]
        public string Get()
        {
            return "Hello";
        }

        [HttpPost]
        public async Task<IActionResult> WebhookAsync(Update update)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await HandleMessageAsync(update);
                        break;
                    case UpdateType.CallbackQuery:
                        await HandleCallbackQueryAsync(update);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return Ok();
        }

        private async Task HandleMessageAsync(Update update)
        {
            if (update.Message!.Text == "/start")
            {
                _queue.Enqueue(new BotMessage { Id = update.Message.Chat.Id, Text = "Здравствуйте! Введите город" });
                return;
            }

            var userId = update.Message.Chat.Id;

            var cities = await _weatherForecast.GetCityAsync(update.Message.Text!);

            if (cities!.Length == 0)
            {
                _queue.Enqueue(new BotMessage
                {
                    Id = userId,
                    Text = $"Такого города нет",
                });
                return;
            }

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                user = new Models.User()
                {
                    City = cities[0],
                    Id = userId,
                    NotificationInterval = Interval.Inset
                };
                await _db.Users.AddAsync(user);
            }

            user.City = cities[0];

            await _db.SaveChangesAsync();

            _queue.Enqueue(new BotMessage
            {
                Id = userId,
                Text = $"Выбран город: {cities[0]}\nВыберите интервал",
                ReplyMarkup = GetIntervalsKeyboard()
            });
        }

        private static IReplyMarkup GetIntervalsKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("1 минута", "OneMinute") },
                new[] { InlineKeyboardButton.WithCallbackData("1 день", "OneDay") },
                new[] { InlineKeyboardButton.WithCallbackData("1 неделя", "OneWeek") }
            });
        }

        private async Task HandleCallbackQueryAsync(Update update)
        {
            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery!.Id);
            if (Enum.TryParse<Interval>(update.CallbackQuery!.Data, out var res))
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == update.CallbackQuery.From.Id);

                if (user == null)
                {
                    _queue.Enqueue(new BotMessage
                    {
                        Id = update.CallbackQuery.From.Id,
                        Text = "Что-то пошло не так. Введите /start"
                    });
                    return;
                }

                user.NotificationInterval = res;

                await _db.SaveChangesAsync();

                _queue.Enqueue(new BotMessage
                {
                    Id = update.CallbackQuery.From.Id,
                    Text = "Интервал выбран. Ждите погоду!"
                });
            }
            else
            {
                _queue.Enqueue(new BotMessage
                {
                    Id = update.CallbackQuery.From.Id,
                    Text = "Что-то пошло не так. Попробуйте еще раз",
                    ReplyMarkup = GetIntervalsKeyboard()
                });
            }
        }
    }
}
