using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.ModelsDB;
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
