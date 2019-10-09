using blogging.data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace blogging.service
{
    public abstract class BaseService<T> where T : class
    {
        private readonly IRepository<T> repository;

        public BaseService(IRepository<T> repository)
        {
            this.repository = repository;
        }

        public virtual async Task<List<T>> GetAllAsync(string textSearch=null)
        {
            return await repository.GetAllAsync();
        }

        public virtual async Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await repository.WhereAsync(predicate);
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await repository.GetByIdAsync(id);
        }
        public virtual async Task DeleteAsync(T entity)
        {
            await repository.DeleteAsync(entity);
        }

        public virtual async Task InsertAsync(T entity)
        {
            await repository.InsertAsync(entity);

        }

        public virtual async Task UpdateAsync(T entity)
        {
            await repository.UpdateAsync(entity);
        }
    }
}
