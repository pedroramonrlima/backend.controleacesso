using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Entities;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Interfaces.Services;


namespace ControleAcesso.Application.Services
{
    public class GenericService<T> : IGenericService<T> where T : class, IEntity
    {
        protected readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual T Add(T entity)
        {
            try
            {
                return _repository.Create(entity);

            }catch (Exception ex)
            {
                throw new DomainException($"{ResponseMessages.ProblemInsertDatabase}");
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                return await _repository.CreateAsync(entity);

            }catch(Exception ex)
            {
                throw new DomainException($"{ResponseMessages.ProblemInsertDatabase}");
            }
        }

        public async Task<T> Delete(int id)
        {
            try
            {
                var genericObject = await GetByIdAsync(id);
                return _repository.Delete(genericObject!);
            }catch(Exception ex)
            {
                throw new DomainException($"{ResponseMessages.ProblemDeleteDatabase}");
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }catch(Exception ex)
            {
                throw new DomainException($"{ResponseMessages.ProblemConsultDatabase}");
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {

            try
            {
                var entity = await _repository.GetAsync(e => e.Id == id) ??
                    throw new DomainException(ResponseMessages.DataNotFound);

                return entity;
            }catch(Exception ex)
            {
                throw new DomainException(ex.Message);
            }
        }

        public virtual T Update(T entity)
        {
            try
            {
                return _repository.Update(entity);
            }catch (Exception ex)
            {
                throw new DomainException($"{ResponseMessages.ProblemUpdateDatabase}");
            }
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            try
            {
                var obj = await _repository.GetAsync(e => e.Id == entity.Id) ??
                    throw new DomainException(ResponseMessages.DataNotFound);
                return await _repository.UpdateAsync(entity);
            }catch(DomainException e)
            {
                throw new DomainException(e.Message);
            }
            catch(Exception ex)
            {
                throw new DomainException($"{ResponseMessages.ProblemUpdateDatabase}");
            }
        }
    }
}
