using AgroBot.Models;
using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.ModelsDB;
using AgroBot.Models.ModelsDto;
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
    public class RouteServiceTest
    {
        private Mock<IRouterRepository> _routeRepository;
        private IRouterRepository _mockRouteRepository;
        private IMapper _mapper;
        private List<Route> _fakeRoute;
        private List<RouteDto> _fakeRouteDto;
        private RouteService _routeService;
        private static Random _random;
        public RouteServiceTest()
        {
            GenerateData();
        }

        [Fact]
        public async Task<List<Route>> GetAll_ListRoute()
        {
            GetMockRouteService();
            var user = await _routeService.GetAllAsync(default(CancellationToken));

            Assert.Equal(user.Count(), _fakeRoute.Count());
            return (List<Route>)user;
        }
        [Fact]
        public async Task<Route> Guid_GetById_Route()
        {
            GetMockRouteService();
            var user = await _routeService.GetByIdAsync(_fakeRoute[0].Id, default(CancellationToken));

            Assert.NotNull(user);
            return user;
        }
        [Fact]
        public async Task<Route> Route_Remove_Route()
        {
            GetMockRouteService();
            var user = await _routeService.DeleteAsync(_fakeRoute[0].Id, default(CancellationToken));

            Assert.NotNull(user);
            return user;
        }
        [Fact]
        public async Task<Route> Route_Add_Route()
        {
            GetMockRouteService();
            var user = await _routeService.AddAsync(_fakeRoute[0], default(CancellationToken));

            Assert.NotNull(user);
            return user;
        }
        [Fact]
        public async Task<Route> Route_Update_Route()
        {
            GetMockRouteService();
            var user = await _routeService.UpdateAsync(_fakeRoute[0].Id, _fakeRoute[0], default(CancellationToken));

            Assert.NotNull(user);
            return user;
        }
        [Fact]
        public async Task<IList<Route>> DriverChatId_long_GetRouteByDriverChatId_IList_Route()
        {
            GetMockRouteService();
            var user = await _routeService.GetRouteByDriverChatId(_fakeRoute[0].DriverChatId);

            Assert.NotNull(user);
            return user;
        }
        [Fact]
        public async Task<IList<Route>> LogicChatId_long_GetRouteByLogistChatId_IList_Route()
        {
            GetMockRouteService();
            var user = await _routeService.GetRouteByLogistChatId(_fakeRoute[0].LogicChatId);

            Assert.NotNull(user);
            return user;
        }

        [Fact]
        public async void ListRouteDto_long_InsertMany_Task()
        {
            GetMockRouteService();
            await _routeService.InsertMany(_fakeRouteDto, _fakeRoute[0].LogicChatId);

        }

        


        private void GetMockRouteService()
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
                _routeRepository = new Mock<IRouterRepository>();
                _routeRepository.Setup(s => s.GetAllAsync(default(CancellationToken))).ReturnsAsync(_fakeRoute);
                _routeRepository.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default(CancellationToken))).ReturnsAsync(_fakeRoute[0]);
                _routeRepository.Setup(s => s.RemoveAsync(It.IsAny<Guid>(), default(CancellationToken))).ReturnsAsync(_fakeRoute[0]);
                _routeRepository.Setup(s => s.AddAsync(It.IsAny<Route>(), default(CancellationToken)));
                _routeRepository.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Route>(), default(CancellationToken)));

                _routeRepository.Setup(s => s.InsertMany(It.IsAny<List<Route>>(), default(CancellationToken)));
                _routeRepository.Setup(s => s.GetFilteredRoutes(It.IsAny<RouteFilter>(), default(CancellationToken))).ReturnsAsync(_fakeRoute);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            _mockRouteRepository = _routeRepository.Object;
            _routeService = new RouteService(_mockRouteRepository, _mapper);

        }
        private void GenerateData()
        {
            _random = new Random();
            _fakeRoute = new List<Route>();
            _fakeRouteDto = new List<RouteDto>();
            for (int i = 0; i < 10; i++)
            {
                _fakeRouteDto.Add(new RouteDto
                {
                    CreatedDate = DateTime.Now,
                    Goods = RandomString(5),
                    Kilo = _random.Next(1, 12365),
                    Name = RandomString(5),
                    Points = new List<CheckPointDto>
                    {
                         new CheckPointDto
                         {
                             Name = RandomString(45),
                             IsFullfil = false
                         }
                    }

                });

                _fakeRoute.Add(
                    new Route
                    {
                        Id = Guid.NewGuid(),
                        AppointDate = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        DriverChatId = _random.Next(1, 1265000),
                        FullffilDate = DateTime.Now,
                        Goods = RandomString(15),
                        IsDeleted = false,
                        Kilo = _random.Next(1, 126312),
                        LogicChatId = _random.Next(1, 65461321),
                        Name = RandomString(10),
                        Points = new List<CheckPoint>
                        {
                        new CheckPoint
                        {
                            Id = Guid.NewGuid(),
                            Name = RandomString(51),
                            IsFullfil = false
                        }
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
