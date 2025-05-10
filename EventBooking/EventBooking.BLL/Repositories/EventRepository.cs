using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBooking.BLL.Repositories.Contract;
using EventBooking.DAL.Data;
using EventBooking.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.BLL.Repositories
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        private readonly EventBookingDbContext _context;
        private readonly DbSet<Event> _dbSet;
        public EventRepository(EventBookingDbContext context) : base(context)
        {
           this._context = context;
            _dbSet = context.Set<Event>();
        }
        public async Task<int> GetFilteredEventsCount(int? categoryId, decimal? maxPrice, decimal? minPrice, DateTime? startDate, DateTime? endDate)
        {
            IQueryable<Event> query = _context.Events.Where(e => !e.IsDeleted);

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);
            if (maxPrice.HasValue)
                query = query.Where(e => e.Price <= maxPrice.Value);
            if (minPrice.HasValue)
                query = query.Where(e => e.Price >= minPrice.Value);
            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<List<Event>> GetEventsWithFilters(int pageSize, int pageIndex, int? categoryId, decimal? maxPrice, decimal? minPrice, DateTime? startDate, DateTime? endDate)
        {
            IQueryable<Event> query = _context.Events
                .Where(e => !e.IsDeleted)
                .Include(e => e.Category)
                .Include(e => e.EventPhotos);

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);
            if (maxPrice.HasValue)
                query = query.Where(e => e.Price <= maxPrice.Value);
            if (minPrice.HasValue)
                query = query.Where(e => e.Price >= minPrice.Value);
            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value);

            return await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Event> GetByIdWithIncludeAsync(int id, params string[] includeProperties)
        {
            IQueryable<Event> query = _context.Events.AsQueryable();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<Event> GetByIdIncludingDeletedAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
    }
}
