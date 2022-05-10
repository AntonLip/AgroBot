using AgroBot.Models.Interfaces;
using AgroBot.Models.Interfaces.IService;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgroBot.Services
{
    public abstract class BaseService<TModel, TModelDto, TModelAddDto, TModelUpdateDto> : IService<TModel, TModelDto, TModelAddDto, TModelUpdateDto, Guid>
      where TModel : IEntity<Guid>
            where TModelDto : IEntity<Guid>
            where TModelAddDto : IEntity<Guid>
            where TModelUpdateDto : IEntity<Guid>
    {
        protected readonly IRepository<TModel, Guid> _repository;
        protected readonly IMapper _mapper;
        public BaseService(IRepository<TModel, Guid> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<TModelDto> AddAsync(TModelAddDto modelDto, CancellationToken cancellationToken = default)
        {
            if (modelDto is null)
                throw new ArgumentNullException();

            var model = _mapper.Map<TModel>(modelDto);

            await _repository.AddAsync(model, cancellationToken);
            return model is null ? throw new ArgumentNullException() : _mapper.Map<TModelDto>(model);
        }

        public async Task<TModelDto> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException();

            var model = await _repository.RemoveAsync(id, cancellationToken);

            if (model is null)
                throw new ArgumentException();



            return model is null ? throw new ArgumentNullException() : _mapper.Map<TModelDto>(model);
        }

        public virtual async Task<IEnumerable<TModelDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var listModelDto = await _repository.GetAllAsync(cancellationToken);

            return listModelDto is null ? throw new ArgumentException() : _mapper.Map<List<TModelDto>>(listModelDto);
        }

        public virtual async Task<TModelDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException();

            var modelDto = await _repository.GetByIdAsync(id, cancellationToken);

            return modelDto is null ? throw new ArgumentException() : _mapper.Map<TModelDto>(modelDto);
        }

        public async Task<TModelDto> UpdateAsync(Guid Id, TModelUpdateDto modelDto, CancellationToken cancellationToken = default)
        {
            if (Id != modelDto.Id)
                throw new ArgumentNullException();

            var modeldb = _mapper.Map<TModel>(modelDto);

             await _repository.UpdateAsync(modeldb.Id, modeldb, cancellationToken);

            return modeldb is null ? throw new ArgumentNullException() : _mapper.Map<TModelDto>(modeldb); 
        }
    }


}
