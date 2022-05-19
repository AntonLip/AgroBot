using AgroBot.DataAccess;
using AgroBot.Models;
using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.Interfaces.IService;
using AgroBot.Models.ModelsDB;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<int> InsertMany(List<RouteDto> route, CancellationToken cancellationToken = default)
        {
            if (route is null)
                throw new ArgumentException();

            var model = _mapper.Map<List<Route>>(route);

            await _routeRepository.InsertMany(model, cancellationToken);
            return route.Count;
        }
    }
}
