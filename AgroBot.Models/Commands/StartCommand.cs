using AgroBot.Models.Interfaces;
using AgroBot.Models.Interfaces.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AgroBot.Models.Commands
{
    internal class StartCommand : ICommand
    {
        private readonly IUserService _userService;
        public StartCommand(IUserService userService)
        {
            _userService = userService;
        }
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
            var chatId = message.Chat.Id;
            List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();
            var user = await _userService.GetByChatIdAsync(chatId);
            if (user is null || user.Role is null)
            {
                InlineKeyboardButton registerButton = new InlineKeyboardButton("Регистрация");
                registerButton.CallbackData = @"/register";
                var registerList = new List<InlineKeyboardButton>();
                registerList.Add(registerButton);
                keybord.Add(registerList);
            }
            else
            {
                if (user.Role.Contains("Logist"))
                {                    
                    InlineKeyboardButton logistButton = new InlineKeyboardButton("Управление маршрутами");
                    logistButton.CallbackData = @"/routes";
                    var logistList = new List<InlineKeyboardButton>();
                    logistList.Add(logistButton);
                    keybord.Add(logistList);
                }
                if (user.Role.Contains("Driver"))
                {
                    InlineKeyboardButton driverButton = new InlineKeyboardButton("Водители");
                    driverButton.CallbackData = @"/driver";
                    var driverList = new List<InlineKeyboardButton>();
                    driverList.Add(driverButton);
                    keybord.Add(driverList);
                }
                if (user.Role.Contains("Manager"))
                {
                    InlineKeyboardButton managerButton = new InlineKeyboardButton("Контроль маршрутов");
                    managerButton.CallbackData = @"/manager";
                    var managerList = new List<InlineKeyboardButton>();
                    managerList.Add(managerButton);
                    keybord.Add(managerList);
                }
                if (user.Role.Contains("Admin"))
                {
                    InlineKeyboardButton registerButton = new InlineKeyboardButton("Работа с пользователями");
                    registerButton.CallbackData = @"/register";
                    var registerList = new List<InlineKeyboardButton>();
                    registerList.Add(registerButton);
                    keybord.Add(registerList);
                }
            }
            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);

            await client.SendTextMessageAsync(chatId, "Главное меню", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
        }

        public Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }
    }
}
