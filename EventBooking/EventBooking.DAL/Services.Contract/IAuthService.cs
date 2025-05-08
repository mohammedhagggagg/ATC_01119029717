using EventBooking.DAL.Models;

namespace EventBooking.DAL.Services.Contract
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}
