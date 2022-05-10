using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.ModelsDB;
using AgroBot.Models.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.DataAccess
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(IOptions<MongoDBSettings> mongoDbSettings)
            :base(mongoDbSettings)
        {

        }

        public  Task<ApplicationUser> GetByChatId(long chatId, CancellationToken cancellationToken = default)
        {
           return  GetCollection().Find(Builders<ApplicationUser>.Filter.Eq(new ExpressionFieldDefinition<ApplicationUser, long>(x => x.ChatId), chatId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public Task<List<ApplicationUser>> GetUnregistredUsers(CancellationToken cancellationToken)
        {
            return GetCollection().Find(Builders<ApplicationUser>.Filter.Eq(new ExpressionFieldDefinition<ApplicationUser, bool>(x => x.IsRegistred), false)).ToListAsync(cancellationToken: cancellationToken);
        }

        public Task<List<ApplicationUser>> GetUserInRole(string role, CancellationToken cancellationToken)
        {
            return GetCollection().Find(Builders<ApplicationUser>.Filter.Eq(new ExpressionFieldDefinition<ApplicationUser, string>(x => x.Role[0]), role)).ToListAsync(cancellationToken: cancellationToken);

        }
    }
}
