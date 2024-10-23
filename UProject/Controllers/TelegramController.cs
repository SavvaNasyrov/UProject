using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UProject.Services;

namespace UProject.Controllers
{
    [Route("/")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;

        private readonly AppDbContext _db;

        public TelegramController(ITelegramBotClient botClient, AppDbContext db)
        {
            _botClient = botClient;
            _db = db;
        }

        [HttpGet]
        public string Get()
        {
            return "Hello";
        }

        [HttpPost]
        public IActionResult Webhook(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    HandleMessage(update);
                    break;
                case UpdateType.CallbackQuery:
                    HandleCallbackQuery(update);
                    break;
            }
            return Ok();
        }

        private void HandleMessage(Update update)
        {
            if (update.Message!.Text == "/start")
            {
                _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Здравствуйте! Введите город");
            }


        }

        private void HandleCallbackQuery(Update update)
        {

        }
    }
}
