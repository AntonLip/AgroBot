using AgroBot.Models.Interfaces;
using AgroBot.Models.Interfaces.IService;
using AgroBot.Models.ModelsDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AgroBot.Models.Commands
{
    public class RouteCommand : ICommand
    {
        public string Name => @"/routes";

        private readonly IUserService _userService;
        private readonly IRouteService _routeService;
        public RouteCommand(IUserService userService, IRouteService routeService)
        {
            _userService = userService;
            _routeService = routeService;
        }
        public bool Contains(Message message)
        {
            if (message is null)
                return false;
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text || message.Type != Telegram.Bot.Types.Enums.MessageType.Document)
            {
                if (message.Text is null)
                {
                    if (message.Caption.Contains(this.Name))
                    {
                        return true;
                    }
                }
                else if (message.Text.Contains(this.Name))
                {
                    return true;
                }

            }

            return false;
        }

        public async Task Execute(Message message, CallbackQuery query, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var user = await _userService.GetByChatIdAsync(chatId);
            if (user is null || user.Role is null || !user.Role.Contains("Logist"))
            {
                await client.SendTextMessageAsync(chatId, "Вам не сюда", Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            else 
            {
                if (message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
                {
                    List<RouteDto> items = null;
                    using (var fileStream = new FileStream(Path.Combine("files", message.Document.FileId), FileMode.Create))
                    {
                        var doc = await client.GetInfoAndDownloadFileAsync(message.Document.FileId, fileStream);
                    }
                    await client.SendTextMessageAsync(chatId, "Файл принят, идет обработка", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    using (var fileStream = new StreamReader(Path.Combine("files", message.Document.FileId)))
                    {
                        var doc = await fileStream.ReadToEndAsync();
                        items = JsonConvert.DeserializeObject<List<RouteDto>>(doc);
                    }
                    if(items is not null)
                    {
                        var cnt = await _routeService.InsertMany(items, chatId);
                        string answ = string.Format("добавлено {0} маршрут(ов)", cnt);
                        await client.SendTextMessageAsync(chatId, answ, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                       
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Ошибка! Проверьте  данные", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                }
                if(message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                {
                    var cmd = message.Text.Split(" ");
                    if (cmd.Length == 1)
                    {
                        var routes = await _routeService.GetRouteByLogistChatId(chatId);
                        foreach (var item in routes)
                        {
                            List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();

                            InlineKeyboardButton removeButton = new InlineKeyboardButton("Удалить");
                            removeButton.CallbackData = @"/routes" + " " + item.Id + " " + "Remove";
                            var buttonsList = new List<InlineKeyboardButton>();
                            buttonsList.Add(removeButton);
                            keybord.Add(buttonsList);

                            InlineKeyboardButton infoButton = new InlineKeyboardButton("Подробнее");
                            infoButton.CallbackData = @"/routes" + " " + item.Id + " " + "Info";
                            var infoList = new List<InlineKeyboardButton>();
                            infoList.Add(infoButton);
                            keybord.Add(infoList);

                            var buttonsAssignList = new List<InlineKeyboardButton>();
                            InlineKeyboardButton assignButton = new InlineKeyboardButton("Назначить водителя");
                            assignButton.CallbackData = @"/routes" + " " + item.Id + " " + "Assign";
                            buttonsAssignList.Add(assignButton);
                            keybord.Add(buttonsAssignList);

                            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);

                            await client.SendTextMessageAsync(chatId, item.Name + " " + item.Goods, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
                        }
                    }
                    else if (cmd.Length == 3 && cmd[2] == "Info")
                    {
                        Guid routeId = Guid.Empty;
                        Guid.TryParse(cmd[1], out routeId);
                        var route = await _routeService.GetByIdAsync(routeId);
                        ApplicationUser logist = new ApplicationUser();

                        if (route.LogicChatId != 0)
                            logist = await _userService.GetByChatIdAsync(route.LogicChatId);


                        string answ = "Маршрут " + route.Name + "\n"
                                    + "Товар: " + route.Goods + "\n"
                                    + "Вес: " + route.Kilo + "\n"
                                    + "Дата добавления: " + route.CreatedDate + "\n"
                                    + "Логист:" + logist.LastName + " "+ logist.FirstName + "\n";
                        if (route.AppointDate != DateTime.MinValue)
                        {
                            ApplicationUser driver = new ApplicationUser();
                            if (route.DriverChatId != 0)
                                driver = await _userService.GetByChatIdAsync(route.DriverChatId);
                            answ += "Дата назначения водителя: " + route.AppointDate + "\n"
                                 + "Водитель: " + driver.LastName + driver.FirstName + "\n";
                        }
                        else
                        {
                            answ += "Водитель не назначен\n";
                        }

                        if (route.FullffilDate != DateTime.MinValue)
                        {
                            answ += "Маршрут выполнен: " + route.CreatedDate + "\n";
                        }
                        else
                        {
                            if (route.Points.Any(x => x.IsFullfil == true))
                            {
                                foreach (var item in route.Points)
                                {
                                    if(item.IsFullfil)
                                    {
                                        answ += "Точка " + item.Name + " пройдена\n";
                                    }
                                }
                            }
                            else 
                            {
                                answ += "Ни одной точки не пройдено\n";
                            }
                        }
                        await client.SendTextMessageAsync(chatId, answ, Telegram.Bot.Types.Enums.ParseMode.Markdown);

                    }
                    else if (cmd.Length == 3 && cmd[2] == "Remove")
                    {
                        Guid guid = Guid.Empty;
                        Guid.TryParse(cmd[1], out guid);
                        await _routeService.DeleteAsync(guid);
                        await client.SendTextMessageAsync(chatId, "Маршрут удален", Telegram.Bot.Types.Enums.ParseMode.Markdown);

                    }
                    else if (cmd.Length == 3 && cmd[2] == "Assign")
                    {
                        var drivers = await _userService.GetUserInRole("Driver");
                        if (drivers is null || drivers.Count == 0)
                            await client.SendTextMessageAsync(chatId, "Нет свободных водителей", Telegram.Bot.Types.Enums.ParseMode.Markdown);

                        List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();
                        foreach (var item in drivers)
                        {
                            InlineKeyboardButton driverButton = new InlineKeyboardButton(item.FirstName + item.LastName);
                            driverButton.CallbackData = @"/routes" + " " + cmd[1] + " " + "Assign" + " " + item.ChatId;
                            var buttonsList = new List<InlineKeyboardButton>();
                            buttonsList.Add(driverButton);
                            keybord.Add(buttonsList);
                        }
                        InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keybord);
                        await client.SendTextMessageAsync(chatId, "Выберите водителя для маршрута", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);

                    }
                    else if (cmd.Length == 4 && cmd[2] == "Assign")
                    {
                        Guid routeId = Guid.Empty;
                        Guid.TryParse(cmd[1], out routeId);
                        var route = await _routeService.GetByIdAsync(routeId);

                        long driverChatId = 0;
                        long.TryParse(cmd[3], out driverChatId);
                        if (driverChatId != 0)
                        {
                            route.DriverChatId = driverChatId;
                            route.AppointDate = DateTime.Now;
                            var driver = await _userService.GetByChatIdAsync(driverChatId);
                            await _routeService.UpdateAsync(routeId, route);
                            var answ = string.Format("На маршрут {0} назначен {1} {2}", route.Name, driver.LastName, driver.FirstName);
                            await client.SendTextMessageAsync(chatId, answ, Telegram.Bot.Types.Enums.ParseMode.Markdown);

                        }
                        else
                            await client.SendTextMessageAsync(chatId, "Обратитесь к админу", Telegram.Bot.Types.Enums.ParseMode.Markdown);


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
