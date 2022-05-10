using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.ModelsDB;
using AgroBot.Models.Settings;
using Microsoft.Extensions.Options;

namespace AgroBot.DataAccess
{
    public class RouteRepository : BaseRepository<Route>, IRouterRepository
    {
        public RouteRepository(IOptions<MongoDBSettings> mongoDbSettings)
            :base(mongoDbSettings)
        {

        }
    }
}
