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

        /// <summary>
        /// Get all objects
        /// </summary>
        /// <param name="textSearch"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAsync(string textSearch=null)
        {
            return await repository.GetAllAsync();
        }

        /// <summary>
        /// Get all objects that satisfy a condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await repository.WhereAsync(predicate);
        }

        /// <summary>
        /// Get an object by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Delete an object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(T entity)
        {
            await repository.DeleteAsync(entity);
        }

        /// <summary>
        /// Insert an object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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
