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
    public class DriverCommand : ICommand
    {
        public string Name => @"/driver";
        private readonly IUserService _userService;
        private readonly IRouteService _routeService;
        public DriverCommand(IUserService userService, IRouteService routeService)
        {
            _userService = userService;
            _routeService = routeService;
        }
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
            var user = await _userService.GetByChatIdAsync(chatId);
            if (user is null || user.Role is null || !user.Role.Contains("Driver"))
            {
                await client.SendTextMessageAsync(chatId, "Вам не сюда", Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            else
            {
                var cmd = message.Text.Split(" ");
                if (cmd.Length == 1)
                {                   
                        List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();

                        InlineKeyboardButton infoButton = new InlineKeyboardButton("ЧП");
                        infoButton.CallbackData = @"/driver" + " " + "Emergency";
                        var infoList = new List<InlineKeyboardButton>();
                        infoList.Add(infoButton);
                        keybord.Add(infoList);

                        var buttonsAssignList = new List<InlineKeyboardButton>();
                        InlineKeyboardButton assignButton = new InlineKeyboardButton("Маршруты");
                        assignButton.CallbackData = @"/driver" + " " + "Routes";
                        buttonsAssignList.Add(assignButton);
                        keybord.Add(buttonsAssignList);

                        InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);

                        await client.SendTextMessageAsync(chatId, "Выберите раздел", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);                    
                }
            }
        }

        public Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }
    }
}
