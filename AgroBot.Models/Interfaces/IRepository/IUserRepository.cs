using AgroBot.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.Models.Interfaces.IRepository
{
    public interface IUserRepository : IRepository<ApplicationUser, Guid>
    {
        Task<ApplicationUser> GetByChatId(long chatId, CancellationToken cancellationToken = default);
        Task<List<ApplicationUser>> GetUnregistredUsers(CancellationToken cancellationToken);
        Task<List<ApplicationUser>> GetUserInRole(string role, CancellationToken cancellationToken);
    }
}
