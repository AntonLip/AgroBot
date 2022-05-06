using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.Models.Interfaces
{
    public interface IRepository<TModel, TId>
       where TModel : IEntity<TId>
    {
        Task AddAsync(TModel obj, CancellationToken cancellationToken = default);
        Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TModel> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task UpdateAsync(TId id, TModel obj, CancellationToken cancellationToken = default);
        Task<TModel> RemoveAsync(TId id, CancellationToken cancellationToken = default);

    }
}
