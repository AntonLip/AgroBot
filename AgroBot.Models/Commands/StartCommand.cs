using AgroBot.Models.Interfaces;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AgroBot.Models.Commands
{
    internal class StartCommand : ICommand
    {
        public string Name => @"/start";

        public bool Contains(Message message)
        {
            if (message is null)
                return false;
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;
            return message.Text.Contains(this.Name);
        }

        public async Task Execute(Message message, CallbackQuery query, TelegramBotClient client)
        {
            long ids = 349548790;
            var chatId = message.Chat.Id;
            var location = message.Chat.Location;
            var yourIds = message.From.Id;
            string answ;
            if (location is not null)
                answ = String.Format("your location is {0}", location.ToString());
            else
                answ = "hi" + yourIds.ToString();
            await client.SendTextMessageAsync(chatId, answ, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            await client.SendTextMessageAsync(ids, answ, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }

        public Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }
    }
}
