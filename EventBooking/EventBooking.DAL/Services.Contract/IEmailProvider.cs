using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBooking.DAL.Services.Contract
{
    public interface IEmailProvider
    {
        Task<int> SendResetCode(string Email);
        Task<string> SendConfirmAccount(string Email, string UrlConfirmation);
    }
}
