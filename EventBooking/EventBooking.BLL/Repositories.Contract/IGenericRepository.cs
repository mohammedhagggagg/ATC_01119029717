using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EventBooking.DAL.Models;

namespace EventBooking.BLL.Repositories.Contract
{
    public interface IGenericRepository <T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(
                                        Expression<Func<T, bool>> filter = null,
                                        string includeProperties = "");
        Task<T?> GetByIdWithIncludeAsync(int id, string includeProperties);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }
}
