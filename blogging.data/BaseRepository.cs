using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

namespace blogging.data
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> dbSet;
        protected AppDbContext dbContext;
        public BaseRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }

        /// <summary>
        /// Get all objects asynchronously
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync(); 
        }

        /// <summary>
        /// Get all objects that satisfy a condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> WhereAsync(Expression<Func<T,bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Get an object by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        /// <summary>
        /// Delete an object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Insert an object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task InsertAsync(T entity)
        {
            dbSet.Add(entity);
            await dbContext.SaveChangesAsync();

        }

        /// <summary>
        /// Update an object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(T entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

    }
}
