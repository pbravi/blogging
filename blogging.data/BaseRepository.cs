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

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync(); 
        }

        public virtual async Task<List<T>> WhereAsync(Expression<Func<T,bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }
        public virtual async Task DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task InsertAsync(T entity)
        {
            dbSet.Add(entity);
            await dbContext.SaveChangesAsync();

        }

        public virtual async Task UpdateAsync(T entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

    }
}
