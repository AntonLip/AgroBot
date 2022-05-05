using AgroBot.Models.Commands;
using AgroBot.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroBot.Models
{
    public class Bot : IBot
    {
        private List<ICommand> commandsList;
        public Bot( )
        {
            commandsList = new List<ICommand>();
            commandsList.Add(new StartCommand());
        }

        public List<ICommand> GetCommands()
        {
            if (commandsList is null)
                throw new System.Exception();
            return commandsList;
        }
    }
}
