using AgroBot.Models.Interfaces;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AgroBot.Models.Commands
{
    public class DriverCommand : ICommand
    {
        public string Name => @"/driver";

        public bool Contains(Message message)
        {
            if (message is null)
                return false;
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;
            return message.Text.Contains(this.Name);
        }

        public Task Execute(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }

        public Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }
    }
}
