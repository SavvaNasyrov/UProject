using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public TelegramController(ITelegramBotClient botClient, AppDbContext db, ILogger<TelegramBotClient> logger)
        {
            _botClient = botClient;
            _db = db;
            _logger = logger;
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
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Здравствуйте! Введите город");
            }

            var userId = update.Message.Chat.Id;

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                user = new Models.User()
                {
                    City = update.Message.Text!,
                    Id = userId,
                    NotificationInterval = Interval.Inset
                };
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
            }

            await _botClient.SendTextMessageAsync(
                userId,
                "Выберите интервал",
                replyMarkup: GetIntervalsKeyboard());
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
            if (Enum.TryParse<Interval>(update.CallbackQuery!.Data, out var res))
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == update.CallbackQuery.From.Id);

                if (user == null)
                {
                    await _botClient.SendTextMessageAsync(
                        update.CallbackQuery.From.Id,
                        "Что-то пошло не так. Введите /start");
                    return;
                }

                user.NotificationInterval = res;

                await _db.SaveChangesAsync();

                await _botClient.SendTextMessageAsync(
                    update.CallbackQuery.From.Id,
                    "Данные успешно сохранены");
            }
            else
            {
                await _botClient.SendTextMessageAsync(
                    update.CallbackQuery.From.Id,
                    "Что-то пошло не так. Попробуйте еще раз",
                    replyMarkup: GetIntervalsKeyboard());
            }
        }
    }
}
