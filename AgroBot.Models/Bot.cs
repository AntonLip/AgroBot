using AgroBot.Models.Commands;
using AgroBot.Models.Interfaces;
using AgroBot.Models.Interfaces.IService;
using System.Collections.Generic;

namespace AgroBot.Models
{
    public class Bot : IBot
    {
        private List<ICommand> commandsList;
        public Bot(IUserService userService, IRouteService routeService, IReportService reportService)
        {
            commandsList = new List<ICommand>();
            commandsList.Add(new StartCommand(userService));
            commandsList.Add(new RegisterCommand(userService));
            commandsList.Add(new RouteCommand(userService, routeService));
            commandsList.Add(new DriverCommand(userService, routeService));
            commandsList.Add(new ManagerCommand(userService, routeService, reportService));
        }
        

        public List<ICommand> GetCommands()
        {
            if (commandsList is null)
                throw new System.Exception();
            return commandsList;
        }
    }
}
