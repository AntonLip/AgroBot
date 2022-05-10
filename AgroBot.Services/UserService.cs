using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.Interfaces.IService;
using AgroBot.Models.ModelsDB;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.Services
{
    public class UserService : BaseService<ApplicationUser, ApplicationUser, ApplicationUser, ApplicationUser>, IUserService

    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository, IMapper mapper)
            :base(userRepository, mapper)
        {
            _userRepository = userRepository;
        }
        public async Task<ApplicationUser> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByChatId(chatId, cancellationToken);

            return user;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUnregistredUsers(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetUnregistredUsers(cancellationToken);
                return users;
        }

        public async Task<List<ApplicationUser>> GetUserInRole(string role, CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetUserInRole(role, cancellationToken);
            return users;
        }

        public async Task<bool> IsInRoleAsync(long chatId, string roleName, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByChatId(chatId, cancellationToken);
            if (user is null)
                throw new ArgumentNullException();
            foreach (var item in user.Role)
            {
                if (item == roleName)
                    return true;
            }
            return false;
        }

        public async Task<ApplicationUser> SetRoleAsync(long chatId, string role, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByChatId(chatId, cancellationToken);
            if (user is null)
                throw new ArgumentNullException();
            if (user.Role is null)
            {
                user.Role = new List<string>();
                user.Role.Add(role);
                user.IsRegistred = true;
            }
            else if (!await IsInRoleAsync(chatId, role, cancellationToken))
            {
                user.Role.Add(role);
                user.IsRegistred = true;
            }
            await _userRepository.UpdateAsync(user.Id, user, cancellationToken);
            return user;
        }
    }
}
