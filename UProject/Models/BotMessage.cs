using Telegram.Bot.Types.ReplyMarkups;

namespace UProject.Models
{
    public record BotMessage
    {
        public required long Id { get; init; }

        public required string Text { get; init; }

        public IReplyMarkup? ReplyMarkup { get; init; }
    }
}
