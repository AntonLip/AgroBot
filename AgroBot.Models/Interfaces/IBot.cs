using System.Collections.Generic;

namespace AgroBot.Models.Interfaces
{
    public interface IBot
    {
        List<ICommand> GetCommands();
    }
}
