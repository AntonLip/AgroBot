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
                        var cnt = await _routeService.InsertMany(items);
                        string answ = string.Format("добавлено {0} маршрут(ов)", cnt);
                        await client.SendTextMessageAsync(chatId, answ, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                       
                    }
                    await client.SendTextMessageAsync(chatId, "Ошибка! Проверьте  данные", Telegram.Bot.Types.Enums.ParseMode.Markdown);

                }
            }
            
        }

        public Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }
    }
}
