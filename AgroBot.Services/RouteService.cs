using AgroBot.Models;
using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.Interfaces.IService;
using AgroBot.Models.ModelsDB;
using AgroBot.Models.ModelsDto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.Services
{
    public class RouteService : BaseService<Route, Route, Route, Route>, IRouteService
    {
        private readonly IMapper _mapper;
        private readonly IRouterRepository _routeRepository;
        public RouteService(IRouterRepository routeRepository, IMapper mapper)
            :base(routeRepository, mapper)
        {
            _routeRepository = routeRepository;
            _mapper = mapper;
        }

        public async Task<IList<Route>> GetRouteByDriverChatId(long chatId)
        {
            if (chatId <= 0)
                throw new ArgumentNullException();
            RouteFilter filter = new RouteFilter { DriverChatId = chatId };
            var routes = await _routeRepository.GetFilteredRoutes(filter);
            return routes is null ? throw new ArgumentException() : routes;
        }

        public async Task<IList<Route>> GetRouteByLogistChatId(long chatId)
        {
            if (chatId <= 0)
                throw new ArgumentNullException();
            RouteFilter filter = new RouteFilter { LogicChatId = chatId };
            var routes = await _routeRepository.GetFilteredRoutes(filter);
            return routes is null ? throw new ArgumentException() : routes;
        }

        public async Task<int> InsertMany(List<RouteDto> route, long logistChatID, CancellationToken cancellationToken = default)
        {
            if (route is null)
                throw new ArgumentException();

            var model = _mapper.Map<List<Route>>(route);
            model.ForEach(x => { x.CreatedDate = DateTime.Now; x.LogicChatId = logistChatID; });
            await _routeRepository.InsertMany(model, cancellationToken);
            return route.Count;
        }
    }
}
