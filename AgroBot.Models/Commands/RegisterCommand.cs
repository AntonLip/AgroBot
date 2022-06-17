using AgroBot.Models.Interfaces;
using AgroBot.Models.Interfaces.IService;
using AgroBot.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AgroBot.Models.Commands
{
    public class RegisterCommand : ICommand
    {
        private readonly IUserService _userService;
        public RegisterCommand(IUserService userService)
        {
            _userService = userService;
        }
        public string Name => @"/register";

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
            var exUser = await _userService.GetByChatIdAsync(chatId);
            if (exUser is null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    ChatId = message.Chat.Id,
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                    Id = Guid.NewGuid(),
                    IsRegistred = false
                };
                var userC = await _userService.AddAsync(user);
                if (userC is null)
                    await client.SendTextMessageAsync(chatId, "Заявка не принята, обратитесь к админу", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                else
                {
                    await client.SendTextMessageAsync(chatId, "Заявка принята", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    var admins = await _userService.GetUserInRole("Admin");
                    foreach (var item in admins)
                    {
                        await client.SendTextMessageAsync(item.ChatId, "новый пользователь", Telegram.Bot.Types.Enums.ParseMode.Markdown);

                    }
                }
            }
            else 
            {
                if (exUser.Role is null)
                    await client.SendTextMessageAsync(chatId, "вы уже подали заявку, ожидайте", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                else
                {
                    if (await _userService.IsInRoleAsync(chatId, "Admin"))
                    {
                        var cmd = message.Text.Split(" ");
                        if(cmd.Length == 1)
                        {
                            var users = await _userService.GetUnregistredUsers();
                            
                            foreach (var item in users)
                            {
                                List<List<InlineKeyboardButton>> inlineKeyboardButtons = new List<List<InlineKeyboardButton>>();
                                InlineKeyboardButton gr442 = new InlineKeyboardButton("Логист");
                                gr442.CallbackData = @"/register" + " " + item.ChatId + " setrole " + "Logist";
                                InlineKeyboardButton gr443 = new InlineKeyboardButton("Админ");
                                gr443.CallbackData = @"/register" + " " + item.ChatId + " setrole " + "Admin";
                                InlineKeyboardButton gr444 = new InlineKeyboardButton("Водитель");
                                gr444.CallbackData = @"/register" + " " + item.ChatId + " setrole " + "Driver";
                                InlineKeyboardButton gr434 = new InlineKeyboardButton("Управление");
                                gr434.CallbackData = @"/register" + " " + item.ChatId + " setrole " + "Manager";
                                InlineKeyboardButton gr435g = new InlineKeyboardButton("Бухгалтер");
                                gr435g.CallbackData = @"/register" + " " + item.ChatId + " setrole " + "Accountant";
                                
                                var list = new List<InlineKeyboardButton>();
                                list.Add(gr442);
                                list.Add(gr443);
                                list.Add(gr444);
                                list.Add(gr435g);
                                list.Add(gr434);
                                inlineKeyboardButtons.Add(list);
                                var list2 = new List<InlineKeyboardButton>();
                                InlineKeyboardButton removeUsers = new InlineKeyboardButton("Удалить пользователя");
                                removeUsers.CallbackData = @"/register" + " " + item.Id + " " + "Remove";
                                list2.Add(removeUsers);                                
                                inlineKeyboardButtons.Add(list2);
                                InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);
                                await client.SendTextMessageAsync(chatId, item.FirstName + " " + item.LastName, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
                            }
                            List<List<InlineKeyboardButton>> keybord = new List<List<InlineKeyboardButton>>();
                            var keybordList = new List<InlineKeyboardButton>();
                            InlineKeyboardButton allUsers = new InlineKeyboardButton("Показать всех пользователей");
                            allUsers.CallbackData = @"/register" + " " + "All";
                            keybordList.Add(allUsers);
                            keybord.Add(keybordList);
                            var keybordMain = new List<InlineKeyboardButton>();
                            InlineKeyboardButton main = new InlineKeyboardButton("Вернуться в главное меню");
                            main.CallbackData = @"/start";
                            keybordMain.Add(main);
                            keybord.Add(keybordMain);
                            InlineKeyboardMarkup KeyboardMarkup = new InlineKeyboardMarkup(keybord);
                            await client.SendTextMessageAsync(chatId, "Меню пользователей", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: KeyboardMarkup);

                        }
                        if (cmd.Length == 2)
                        {
                            var users = await _userService.GetAllAsync();
                            foreach (var item in users)
                            {
                                
                                InlineKeyboardButton gr434 = new InlineKeyboardButton("Удалить");
                                gr434.CallbackData = @"/register" + " " + item.Id + " " + "Remove";
                                List<List<InlineKeyboardButton>> inlineKeyboardButtons = new List<List<InlineKeyboardButton>>();
                                var list = new List<InlineKeyboardButton>();                                
                                list.Add(gr434);
                                inlineKeyboardButtons.Add(list);                                
                                InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);
                                await client.SendTextMessageAsync(chatId, item.FirstName + " " + item.LastName, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
                            }
                        }
                        if (cmd.Length == 3 && cmd[2] == "Remove")
                        {
                            Guid guid = Guid.Empty;
                            Guid.TryParse(cmd[1], out guid);
                            await _userService.DeleteAsync(guid);
                        }
                        await client.SendTextMessageAsync(chatId, "попытка", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        if (cmd.Length == 4)
                        {
                            await client.SendTextMessageAsync(chatId, "попытка изменения роли", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            await _userService.SetRoleAsync(long.Parse(cmd[1]), cmd[2]);
                             await client.SendTextMessageAsync(chatId, "Роль изменена", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        }
                    }
                    else
                    {
                        var ans = string.Format("вы зарегистрированы в качесте");
                        foreach (var r in exUser.Role)
                        {
                            ans += " " + r;
                        }
                        await client.SendTextMessageAsync(chatId, ans, Telegram.Bot.Types.Enums.ParseMode.Markdown);
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
