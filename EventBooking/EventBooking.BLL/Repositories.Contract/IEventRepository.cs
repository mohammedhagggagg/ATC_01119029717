using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBooking.DAL.Models;

namespace EventBooking.BLL.Repositories.Contract
{
    public interface IEventRepository :IGenericRepository<Event>
    {
        Task<int> GetFilteredEventsCount(int? categoryId, decimal? maxPrice, decimal? minPrice, DateTime? startDate, DateTime? endDate);
        Task<List<Event>> GetEventsWithFilters(int pageSize, int pageIndex, int? categoryId, decimal? maxPrice, decimal? minPrice, DateTime? startDate, DateTime? endDate);
        Task<Event> GetByIdWithIncludeAsync(int id, params string[] includeProperties);
        Task<Event> GetByIdIncludingDeletedAsync(int id);

    }
}
