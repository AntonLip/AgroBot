using AgroBot.Models.ModelsDB;
using AgroBot.Models.ModelsDto;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.Models.Interfaces.IRepository
{
    public  interface IRouterRepository : IRepository<Route, Guid>
    {
        Task InsertMany(List<Route> lessons, CancellationToken cancellationToken = default);
        Task<IList<Route>> GetFilteredRoutes(RouteFilter filter, CancellationToken cancellationToken = default);
    }
}
