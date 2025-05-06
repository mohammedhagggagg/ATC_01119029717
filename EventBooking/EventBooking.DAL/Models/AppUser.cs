using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EventBooking.DAL.Models
{
    public class AppUser :IdentityUser
    {
        public string DisplayName { get; set; }
        public string? Photo { get; set; }

        public List<Address>? Addresses { get; set; }
        public bool IsActive { get; set; } = true;


        public int? PasswordResetPin { get; set; } = null;

        public DateTime? ResetExpires { get; set; } = null;
    }
}
