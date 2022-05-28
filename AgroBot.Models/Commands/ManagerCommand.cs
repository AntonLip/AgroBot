using AgroBot.Models.Interfaces;
using AgroBot.Models.Interfaces.IService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace AgroBot.Models.Commands
{
    public class ManagerCommand : ICommand
    {
        public string Name => @"/manager";
        private readonly IUserService _userService;
        private readonly IRouteService _routeService;
        private readonly IReportService _reportService;

        public ManagerCommand(IUserService userService, IRouteService routeService, IReportService reportService)
        {
            _userService = userService;
            _routeService = routeService;
            _reportService = reportService;
        }
        public bool Contains(Message message)
        {
            if (message is null)
                return false;
            if (message.Type != MessageType.Text)
                return false;
            return message.Text.Contains(this.Name);
        }

        public async Task Execute(Message message, CallbackQuery query, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var user = await _userService.GetByChatIdAsync(chatId);
            if (user is null || user.Role is null || !user.Role.Contains("Manager"))
            {
                await client.SendTextMessageAsync(chatId, "Вам не сюда", Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            else
            {
                var cmd = message.Text.Split(" ");
                if (cmd.Length == 1)
                {
                    List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();

                    InlineKeyboardButton removeButton = new InlineKeyboardButton("Отчет по водителям");
                    removeButton.CallbackData = @"/manager" + " " + "driverReport";
                    var buttonsList = new List<InlineKeyboardButton>();
                    buttonsList.Add(removeButton);
                    keybord.Add(buttonsList);

                    var buttonsAssignList = new List<InlineKeyboardButton>();
                    InlineKeyboardButton assignButton = new InlineKeyboardButton("Отчет по логистам");
                    assignButton.CallbackData = @"/manager" + " " + "logistReport";
                    buttonsAssignList.Add(assignButton);
                    keybord.Add(buttonsAssignList);

                    var routeAssignList = new List<InlineKeyboardButton>();
                    InlineKeyboardButton routeButton = new InlineKeyboardButton("Отчет по всем маршрутам");
                    routeButton.CallbackData = @"/manager" + " " + "AllRoute";
                    routeAssignList.Add(routeButton);
                    keybord.Add(routeAssignList);

                    InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);

                    await client.SendTextMessageAsync(chatId, "Выберите тип отчета", parseMode: ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
                }
                else if(cmd.Length == 2)
                {
                    if(cmd[1] == "driverReport")
                    {
                        var drivers = await _userService.GetUserInRole("Driver");
                        if (drivers is null || drivers.Count == 0)
                            await client.SendTextMessageAsync(chatId, "Нет водителей", Telegram.Bot.Types.Enums.ParseMode.Markdown);

                        List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();
                        foreach (var item in drivers)
                        {
                            InlineKeyboardButton driverButton = new InlineKeyboardButton(item.FirstName + item.LastName);
                            driverButton.CallbackData = @"/manager" + " " + "driverReport" + " " + item.ChatId;
                            var buttonsList = new List<InlineKeyboardButton>();
                            buttonsList.Add(driverButton);
                            keybord.Add(buttonsList);

                        }
                        InlineKeyboardButton allDriverButton = new InlineKeyboardButton("Назад");
                        allDriverButton.CallbackData = @"/manager";
                        var AddButtonsList = new List<InlineKeyboardButton>();
                        AddButtonsList.Add(allDriverButton);
                        keybord.Add(AddButtonsList);
                        InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);
                        await client.SendTextMessageAsync(chatId, "Выберите водителя", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
                    }
                    if (cmd[1] == "logistReport")
                    {
                        var drivers = await _userService.GetUserInRole("Logist");
                        if (drivers is null || drivers.Count == 0)
                            await client.SendTextMessageAsync(chatId, "Нет логистов", Telegram.Bot.Types.Enums.ParseMode.Markdown);

                        List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();
                        foreach (var item in drivers)
                        {
                            InlineKeyboardButton driverButton = new InlineKeyboardButton(item.FirstName + item.LastName);
                            driverButton.CallbackData = @"/manager" + " " + "logistReport" + " " + item.ChatId;
                            var buttonsList = new List<InlineKeyboardButton>();
                            buttonsList.Add(driverButton);
                            keybord.Add(buttonsList);

                        }
                        InlineKeyboardButton allDriverButton = new InlineKeyboardButton("Назад");
                        allDriverButton.CallbackData = @"/manager";
                        var AddButtonsList = new List<InlineKeyboardButton>();
                        AddButtonsList.Add(allDriverButton);
                        keybord.Add(AddButtonsList);
                        InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);
                        await client.SendTextMessageAsync(chatId, "Выберите водителя", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
                    }
                    if(cmd[1] == "AllRoute")
                    {
                        var routes = await _routeService.GetAllAsync();
                        var filename = await _reportService.GetReportAllRoutes((IList<ModelsDB.Route>)routes);
                        if(filename.Contains("Exception"))
                        {
                            await client.SendTextMessageAsync(chatId, "Ошибка генерации отчета", ParseMode.Markdown);
                        }
                        else
                        {
                            using (Stream stream = System.IO.File.OpenRead(filename))
                            {
                                await client.SendDocumentAsync(chatId, document: new InputOnlineFile(content: stream, fileName: filename));
                            }
                            _reportService.RemoveReportFile(filename);
                        }
                        
                    }
                }
                else if(cmd.Length == 3)
                {
                    if(cmd[1] == "driverReport")
                    {
                        long driverChatId = 0;
                        long.TryParse(cmd[2], out driverChatId);
                        var driver = await _userService.GetByChatIdAsync(driverChatId);
                        var routes = await _routeService.GetRouteByDriverChatId(driverChatId);
                        var filename = await _reportService.GetReportByDriverAsync(driver, routes);
                        using (Stream stream = System.IO.File.OpenRead(filename))
                        {
                            await client.SendDocumentAsync(chatId, document: new InputOnlineFile(content: stream, fileName: filename));
                        }
                        _reportService.RemoveReportFile(filename);
                    }
                    else if(cmd[1] == "logistReport")
                    {
                        long driverChatId = 0;
                        long.TryParse(cmd[2], out driverChatId);
                        var driver = await _userService.GetByChatIdAsync(driverChatId);
                        var routes = await _routeService.GetRouteByDriverChatId(driverChatId);
                        var filename = await _reportService.GetReportByLogistAsync(driver, routes);
                        using (Stream stream = System.IO.File.OpenRead(filename))
                        {
                            await client.SendDocumentAsync(chatId, document: new InputOnlineFile(content: stream, fileName: filename));
                        }
                        _reportService.RemoveReportFile(filename);
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
