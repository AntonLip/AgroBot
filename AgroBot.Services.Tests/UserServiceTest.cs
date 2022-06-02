using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.Interfaces.IService;
using AgroBot.Models.ModelsDB;
using AgroBot.Models.Settings;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AgroBot.Services.Tests
{
    public class UserServiceTest
    {
        private Mock<IUserRepository> _userRepository;
        private IUserRepository _mockUserRepository;
        private IMapper _mapper;
        private List<ApplicationUser> _fakeUser;
        private UserService _userService;
        private static Random _random;
        public UserServiceTest()
        {
            GenerateData();
        }

        [Fact]
        public async Task<List<ApplicationUser>> GetAll_ListApplicationUsers()
        {
            GetMockUserService();
            var user = await _userService.GetAllAsync(default(CancellationToken));

            Assert.Equal(user.Count(), _fakeUser.Count());
            return (List<ApplicationUser>)user;
        }
        [Fact]
        public async Task<ApplicationUser> Guid_GetById_ApplicationUser()
        {
            GetMockUserService();
            var user = await _userService.GetByIdAsync(_fakeUser[0].Id, default(CancellationToken));

            Assert.NotNull(user);
            return user;
        }
        [Fact]
        public async Task<ApplicationUser> ApplicationUser_Remove_ApplicationUser()
        {
            GetMockUserService();
            var user = await _userService.DeleteAsync(_fakeUser[0].Id, default(CancellationToken));

            Assert.NotNull(user);
            return user;
        }
        [Fact]
        public async Task<ApplicationUser> ApplicationUser_Add_ApplicationUser()
        {
            GetMockUserService();
            var user = await _userService.AddAsync(_fakeUser[0], default(CancellationToken));

            Assert.NotNull(user);
            return user;
        }
        [Fact]
        public async Task<ApplicationUser> ApplicationUser_Update_ApplicationUser()
        {
            GetMockUserService();
            var user = await _userService.UpdateAsync(_fakeUser[0].Id, _fakeUser[0], default(CancellationToken));

            Assert.NotNull(user);
            return user;
        }
        [Fact]
        public async Task<ApplicationUser> ChatId_Long_GetByChatIdAsync_ApplicationUser()
        {
            GetMockUserService();
            var user = await _userService.GetByChatIdAsync(_fakeUser[0].ChatId, default(CancellationToken));

            Assert.NotNull(user);
            return user;
        }
        [Fact]
        public async Task<IEnumerable<ApplicationUser>> Void_GetUnregistredUsers_IEnumerable_ApplicationUsers()
        {
            GetMockUserService();
            var user = await _userService.GetUnregistredUsers(default(CancellationToken));

            Assert.Equal(user.Count(), _fakeUser.Where(p => p.IsRegistred == false).Count());
            return user;
        }
        [Fact]
        public async Task<IEnumerable<ApplicationUser>> Role_String_GetUserInRole_IEnumerable_ApplicationUsers()
        {
            GetMockUserService();
            var user = await _userService.GetUserInRole(_fakeUser[0].Role[0], default(CancellationToken));

            Assert.Equal(user.Count(), _fakeUser.Count());
            return user;
        }
        [Fact]
        public async Task<bool> ChatId_Long_Role_String_GetUserInRole_bool()
        {
            GetMockUserService();
            var user = await _userService.IsInRoleAsync(_fakeUser[0].ChatId, _fakeUser[0].Role[0], default(CancellationToken));
            return user;
        }
        [Fact]
        public async Task<ApplicationUser> ChatId_Long_Role_String_SetRoleAsync_ApplicationUsers()
        {
            GetMockUserService();
            var user = await _userService.SetRoleAsync(_fakeUser[0].ChatId, _fakeUser[0].Role[0], default(CancellationToken));
            Assert.NotNull(user);
            return user;
        }

        private void GetMockUserService()
        {
            var services = new ServiceCollection();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var myProfile = new MapperProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(myProfile);
                cfg.ConstructServicesUsing(serviceProvider.GetService);
            });
            _mapper = new Mapper(configuration);
            try
            {
                _userRepository = new Mock<IUserRepository>();
                _userRepository.Setup(s => s.GetAllAsync(default(CancellationToken))).ReturnsAsync(_fakeUser);
                _userRepository.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default(CancellationToken))).ReturnsAsync(_fakeUser[0]);
                _userRepository.Setup(s => s.GetByChatId(It.IsAny<long>(), default(CancellationToken))).ReturnsAsync(_fakeUser[0]);
                _userRepository.Setup(s => s.GetUserInRole(It.IsAny<string>(), default(CancellationToken))).ReturnsAsync(_fakeUser);
                _userRepository.Setup(s => s.GetUnregistredUsers(default(CancellationToken))).ReturnsAsync(_fakeUser.Where(p => p.IsRegistred == false).ToList());
                _userRepository.Setup(s => s.RemoveAsync(It.IsAny<Guid>(), default(CancellationToken))).ReturnsAsync(_fakeUser[0]);
                _userRepository.Setup(s => s.AddAsync(It.IsAny<ApplicationUser>(), default(CancellationToken)));
                _userRepository.Setup(s => s.UpdateAsync(It.IsAny<Guid>(),  It.IsAny<ApplicationUser>(), default(CancellationToken)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            _mockUserRepository = _userRepository.Object;
            _userService = new UserService(_mockUserRepository, _mapper);

        }
        private void GenerateData()
        {
            _random = new Random();
            _fakeUser = new List<ApplicationUser>();
            for (int i = 0; i < 10; i++)
            {
                _fakeUser.Add(
                    new ApplicationUser
                    {                       
                        Id = Guid.NewGuid(),
                        ChatId = _random.Next(1,100000),
                        FirstName = RandomString(5),
                        LastName = RandomString(8),
                        IsRegistred = false,
                        Role = new List<string> 
                        {
                            RandomString(7)
                        } 
                    });
            }

        }
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
