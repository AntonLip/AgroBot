using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.Models.Interfaces.IService
{
    public interface IService<TModel, TModelDto, TModelAddDto, TModelUpdateDto, TId>
            where TModel : IEntity<TId>
            where TModelDto : IEntity<TId>
            where TModelAddDto : IEntity<TId>
            where TModelUpdateDto : IEntity<TId>
    {
        Task<IEnumerable<TModelDto>> GetAllAsync(CancellationToken cancellationToken = default); 
        Task<TModelDto> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<TModelDto> AddAsync(TModelAddDto modelDto, CancellationToken cancellationToken = default);
        Task<TModelDto> DeleteAsync(TId id, CancellationToken cancellationToken = default);    
        Task<TModelDto> UpdateAsync(TId Id, TModelUpdateDto modelDto, CancellationToken cancellationToken = default);
    }
}
