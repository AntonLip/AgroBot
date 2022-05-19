using AgroBot.Models.Interfaces;
using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.DataAccess
{

    public class BaseRepository<TModel> : IRepository<TModel, Guid>
    where TModel : IEntity<Guid>
    {

        private readonly MongoDBSettings _mongoDbSettings;
        private readonly MongoClient _mongoClient;
        protected readonly IMongoDatabase _database;

        protected private BaseRepository(IOptions<MongoDBSettings> mongoDbSettings)
        {
            _mongoDbSettings = mongoDbSettings.Value;
            _mongoClient = new MongoClient(_mongoDbSettings.ConnectionString);
            _database = _mongoClient.GetDatabase(_mongoDbSettings.DatabaseName);
        }
        public virtual Task AddAsync(TModel obj, CancellationToken cancellationToken = default)
        {
            return GetCollection().InsertOneAsync(obj, cancellationToken);
        }

        public virtual async Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await GetCollection().Find(Builders<TModel>.Filter.Empty).ToListAsync(cancellationToken);
        }

        public virtual async Task<TModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await GetCollection().Find(Builders<TModel>.Filter.Eq(new ExpressionFieldDefinition<TModel, Guid>(x => x.Id), id)).FirstAsync(cancellationToken);
        }

        public virtual Task<TModel> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
        {

            return GetCollection().FindOneAndDeleteAsync<TModel>(Builders<TModel>.Filter.Eq(new ExpressionFieldDefinition<TModel, Guid>(x => x.Id), id), cancellationToken: cancellationToken);
        }

        public virtual Task UpdateAsync(Guid id, TModel obj, CancellationToken cancellationToken = default)
        {
            return GetCollection().ReplaceOneAsync(Builders<TModel>.Filter.Eq(new ExpressionFieldDefinition<TModel, Guid>(x => x.Id), id), obj, new ReplaceOptions(), cancellationToken);

        }
        protected IMongoCollection<TModel> GetCollection()
        {
            return _database.GetCollection<TModel>(typeof(TModel).Name);
        }
    }

}
