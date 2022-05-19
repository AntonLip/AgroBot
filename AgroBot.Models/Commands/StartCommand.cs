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
            var list = new List<List<KeyboardButton>>();
            var user = await _userService.GetByChatIdAsync(chatId);
            if (user is null || user.Role is null)
            {
                var registerButtons = new List<KeyboardButton>();
                registerButtons.Add(new KeyboardButton("/register"));
                list.Add(registerButtons);
            }
            else
            {
                if (user.Role.Contains("Logist"))
                {
                    var buttonsRoutes = new List<KeyboardButton>();
                    buttonsRoutes.Add(new KeyboardButton("/routes"));
                    list.Add(buttonsRoutes);
                    await client.SendTextMessageAsync(chatId, "Для работы с маршрутами нажмите кнопку routes", Telegram.Bot.Types.Enums.ParseMode.Markdown);

                }
                if (user.Role.Contains("Manager"))
                {
                    var buttonsRoutes = new List<KeyboardButton>();
                    buttonsRoutes.Add(new KeyboardButton("/manager"));
                    list.Add(buttonsRoutes);
                    await client.SendTextMessageAsync(chatId, "Для контроля маршрутов manager", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                if (user.Role.Contains("Admin"))
                {
                    var buttonsRoutes = new List<KeyboardButton>();
                    buttonsRoutes.Add(new KeyboardButton("/register"));
                    list.Add(buttonsRoutes);
                    await client.SendTextMessageAsync(chatId, "Для работы с пользователями register", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                if (user.Role.Contains("Driver"))
                {
                    var buttonsRoutes = new List<KeyboardButton>();
                    buttonsRoutes.Add(new KeyboardButton("/driver"));
                    list.Add(buttonsRoutes);
                    await client.SendTextMessageAsync(chatId, "Для работы нажмите drive", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
            }
            
            var startButtons = new List<KeyboardButton>();
            startButtons.Add(new KeyboardButton("/start"));
            list.Add(startButtons);

            var markup = new ReplyKeyboardMarkup(list);
            await client.SendTextMessageAsync(chatId, "Для регистрации нажмите кнопку register", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: markup);
        }

        public Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }
    }
}
