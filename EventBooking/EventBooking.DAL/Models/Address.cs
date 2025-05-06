using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventBooking.DAL.Models
{
    public class Address : BaseEntity
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Region { get; set; }
        public string City { get; set; }

        public string Country { get; set; }
        public string? AddressDetails { get; set; }

        public string AppUserId { get; set; }

        [JsonIgnore]
        public AppUser? AppUser { get; set; }
    }
}
