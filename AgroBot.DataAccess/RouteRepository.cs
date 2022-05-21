using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.ModelsDB;
using AgroBot.Models.ModelsDto;
using AgroBot.Models.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.DataAccess
{
    public class RouteRepository : BaseRepository<Route>, IRouterRepository
    {
        public RouteRepository(IOptions<MongoDBSettings> mongoDbSettings)
            :base(mongoDbSettings)
        {

        }

        public async Task<IList<Route>> GetFilteredRoutes(RouteFilter externalFilter, CancellationToken cancellationToken = default)
        {
            FilterDefinition<Route> filter =
                 Builders<Route>.Filter.Eq(new ExpressionFieldDefinition<Route, bool>(x => x.IsDeleted), false);

           
            if (!String.IsNullOrEmpty(externalFilter?.Name))
            {
                filter &= Builders<Route>.Filter.Eq(x => x.Name, externalFilter.Name);
            }
            if (!String.IsNullOrEmpty(externalFilter?.Goods))
            {
                filter &= Builders<Route>.Filter.Eq(x => x.Goods, externalFilter.Goods);
            }
            if (externalFilter?.Kilo > 0)
            {
                filter &= Builders<Route>.Filter.Eq(x => x.Kilo, externalFilter.Kilo);
            }
            if (externalFilter?.DriverChatId > 0)
            {
                filter &= Builders<Route>.Filter.Eq(x => x.DriverChatId, externalFilter.DriverChatId);
            }
            if (externalFilter?.DriverChatId > 0)
            {
                filter &= Builders<Route>.Filter.Eq(x => x.DriverChatId, externalFilter.DriverChatId);
            }
            if (externalFilter?.LogicChatId > 0)
            {
                filter &= Builders<Route>.Filter.Eq(x => x.LogicChatId, externalFilter.LogicChatId);
            }

            if (externalFilter?.CreatedDateStart != null && externalFilter?.CreatedDateStart != DateTime.MinValue)
            {
                filter &= Builders<Route>.Filter.Gte(x => x.CreatedDate, externalFilter?.CreatedDateStart);
            }
            if (externalFilter?.CreatedDateEnd != null && externalFilter?.CreatedDateEnd != DateTime.MinValue)
            {
                filter &= Builders<Route>.Filter.Lt(x => x.CreatedDate, externalFilter?.CreatedDateEnd);
            }

            if (externalFilter?.AppointDateStart != null && externalFilter?.AppointDateStart != DateTime.MinValue)
            {
                filter &= Builders<Route>.Filter.Gte(x => x.AppointDate, externalFilter?.AppointDateStart);
            }
            if (externalFilter?.AppointDateEnd != null && externalFilter?.AppointDateEnd != DateTime.MinValue)
            {
                filter &= Builders<Route>.Filter.Lt(x => x.AppointDate, externalFilter?.AppointDateEnd);
            }

            if (externalFilter?.FullffilDateStart != null && externalFilter?.FullffilDateStart != DateTime.MinValue)
            {
                filter &= Builders<Route>.Filter.Gte(x => x.FullffilDate, externalFilter?.FullffilDateStart);
            }
            if (externalFilter?.FullffilDateEnd != null && externalFilter?.FullffilDateEnd != DateTime.MinValue)
            {
                filter &= Builders<Route>.Filter.Lt(x => x.FullffilDate, externalFilter?.FullffilDateEnd);
            }

            return await GetCollection().Find(filter).ToListAsync(cancellationToken);
        }

        public async Task InsertMany(List<Route> routes, CancellationToken cancellationToken = default)
        {
           
                var options = new InsertManyOptions { IsOrdered = false, BypassDocumentValidation = false };

                try
                {
                    await GetCollection().InsertManyAsync(routes, options, cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
           
        }
    }
}
