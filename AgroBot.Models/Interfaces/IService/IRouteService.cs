using AgroBot.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.Models.Interfaces.IService
{
    public interface IRouteService : IService<Route, Route, Route, Route, Guid>
    {
        Task<int> InsertMany(List<RouteDto> route, long logistChatID, CancellationToken cancellationToken = default);
        Task<IList<Route>> GetRouteByLogistChatId(long chatId);
    }
}
