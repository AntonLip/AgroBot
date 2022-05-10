using AgroBot.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.Models.Interfaces.IService
{
    public interface IUserService: IService<ApplicationUser, ApplicationUser, ApplicationUser, ApplicationUser, Guid>
    {
        Task<ApplicationUser> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
        Task<bool> IsInRoleAsync(long chatId, string roleName, CancellationToken cancellationToken = default);
        Task<ApplicationUser> SetRoleAsync(long chatId, string role, CancellationToken cancellationToken = default);
        Task<IEnumerable<ApplicationUser>> GetUnregistredUsers(CancellationToken cancellationToken = default);
        Task<List<ApplicationUser>> GetUserInRole(string role, CancellationToken cancellationToken = default);
    }
}
