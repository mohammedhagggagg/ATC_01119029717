using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EventBooking.BLL.Repositories.Contract;
using EventBooking.DAL.Data;
using EventBooking.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.BLL.Repositories
{
    public class GenericRepository <T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly EventBookingDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(EventBookingDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task<T> GetByIdAsync(int id)
        {
            var result = await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id && EF.Property<bool>(e, "IsDeleted") == false);
            return result;
        }
        public async Task<T?> GetByIdWithIncludeAsync(int id, string includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var result = await _dbSet.Where(t => t.IsDeleted == false).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>> filter = null,
        string includeProperties = "")
        {
            IQueryable<T> query = _dbSet.Where(t => !t.IsDeleted);

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.AsNoTracking().ToListAsync();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }
        public async Task DeleteAsync(T entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
