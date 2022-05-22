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

                    var mainList = new List<InlineKeyboardButton>();
                    InlineKeyboardButton mainButton = new InlineKeyboardButton("Главное меню");
                    mainButton.CallbackData = @"/start";
                    mainList.Add(mainButton);
                    keybord.Add(mainList);

                    InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);

                    await client.SendTextMessageAsync(chatId, "Выберите раздел", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
                }
                if (cmd.Length == 2 && cmd[1] == "Emergency")
                {
                    var routes = await _routeService.GetRouteByDriverChatId(chatId);
                    List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();
                    foreach (var item in routes)
                    {
                        var list = new List<InlineKeyboardButton>();
                        InlineKeyboardButton button = new InlineKeyboardButton(item.Name);
                        button.CallbackData = @"/driver" + " " + "Emergency" + " " + item.Id;
                        list.Add(button);
                        keybord.Add(list);
                    }

                    InlineKeyboardButton driverButton = new InlineKeyboardButton("Водители");
                    driverButton.CallbackData = @"/driver";
                    var driverList = new List<InlineKeyboardButton>();
                    driverList.Add(driverButton);
                    keybord.Add(driverList);

                    var mainList = new List<InlineKeyboardButton>();
                    InlineKeyboardButton mainButton = new InlineKeyboardButton("Главное меню");
                    mainButton.CallbackData = @"/start";
                    mainList.Add(mainButton);
                    keybord.Add(mainList);

                    InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);
                    await client.SendTextMessageAsync(chatId, "Выберите проблемный маршрут", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
                }
                if (cmd.Length == 2 && cmd[1] == "Routes")
                {
                    var routes = await _routeService.GetRouteByDriverChatId(chatId);
                    List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();
                    foreach (var item in routes)
                    {
                        var list = new List<InlineKeyboardButton>();
                        InlineKeyboardButton button = new InlineKeyboardButton(item.Name);
                        button.CallbackData = @"/driver" + " " + "Routes" + " " + item.Id;
                        list.Add(button);
                        keybord.Add(list);
                    }

                    InlineKeyboardButton driverButton = new InlineKeyboardButton("Водители");
                    driverButton.CallbackData = @"/driver";
                    var driverList = new List<InlineKeyboardButton>();
                    driverList.Add(driverButton);
                    keybord.Add(driverList);

                    var mainList = new List<InlineKeyboardButton>();
                    InlineKeyboardButton mainButton = new InlineKeyboardButton("Главное меню");
                    mainButton.CallbackData = @"/start";
                    mainList.Add(mainButton);
                    keybord.Add(mainList);

                    InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);
                    await client.SendTextMessageAsync(chatId, "Выберите маршрут", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
                }
                if (cmd.Length == 3 && cmd[1] == "Emergency")
                {
                    Guid routeId;
                    Guid.TryParse(cmd[2], out routeId);
                    var route = await _routeService.GetByIdAsync(routeId);
                    var driver = await _userService.GetByChatIdAsync(chatId);
                    string answ = string.Format("У {0} {1} на маршруте {2} с грузом {3} возникли проблемы, свяжитесь с водителями в районе с целью оказания помощи", driver.FirstName, driver.LastName, route.Name, route.Goods);
                    await client.SendTextMessageAsync(route.LogicChatId, answ, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                if (cmd.Length == 3 && cmd[1] == "Routes")
                {
                    Guid routeId;
                    Guid.TryParse(cmd[2], out routeId);
                    var route = await _routeService.GetByIdAsync(routeId);
                    List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();
                    foreach (var item in route.Points)
                    {
                        if (!item.IsFullfil)
                        {
                            var list = new List<InlineKeyboardButton>();
                            InlineKeyboardButton button = new InlineKeyboardButton(item.Name);
                            button.CallbackData = @"/driver" + " " + "Rout" + " " + route.Id + " " + item.Name;
                            list.Add(button);
                            keybord.Add(list);
                        }
                    }

                    InlineKeyboardButton driverButton = new InlineKeyboardButton("Водители");
                    driverButton.CallbackData = @"/driver";
                    var driverList = new List<InlineKeyboardButton>();
                    driverList.Add(driverButton);
                    keybord.Add(driverList);

                    var mainList = new List<InlineKeyboardButton>();
                    InlineKeyboardButton mainButton = new InlineKeyboardButton("Главное меню");
                    mainButton.CallbackData = @"/start";
                    mainList.Add(mainButton);
                    keybord.Add(mainList);

                    InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);
                    await client.SendTextMessageAsync(route.DriverChatId, "Выберите точку для отметки о выполнении", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
                }
                if (cmd.Length == 4 && cmd[1] == "Rout")
                {
                    Guid routeId;
                    Guid.TryParse(cmd[2], out routeId);
                    string answ = "Водитель отметил контрольную точку(и) на маршруте: \n";
                    int cnt = answ.Length;
                    var route = await _routeService.GetByIdAsync(routeId);
                    var cntFulfilPoint = 0;
                    foreach (var item in route.Points)
                    {
                        if (item.Name == cmd[3])
                        {
                            item.IsFullfil = true;
                            answ += item.Name + " \n";
                            cntFulfilPoint++;
                        }
                    }
                    if (cntFulfilPoint == route.Points.Count)
                    {
                        route.IsDeleted = true;
                        route.FullffilDate = DateTime.Now;
                    }
                    if (cnt == answ.Length)
                        await client.SendTextMessageAsync(route.DriverChatId, "Изменения не прошли", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    else
                    {
                        await _routeService.UpdateAsync(routeId, route);
                        if (route.IsDeleted)
                        {
                            answ = string.Format("маршрут {0} отмечен как исполненный в {1}", route.Name, route.FullffilDate);
                            await client.SendTextMessageAsync(route.LogicChatId, answ, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        }
                        else
                        {
                            await client.SendTextMessageAsync(route.LogicChatId, answ, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        }
                        await client.SendTextMessageAsync(route.DriverChatId, "Изменения приняты", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                }
            }
        }

        public Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }

    }
}
