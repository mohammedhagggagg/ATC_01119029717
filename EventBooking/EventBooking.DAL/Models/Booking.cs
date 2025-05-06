using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBooking.DAL.Models
{
    public class Booking :BaseEntity
    {
        //public DateTime BookingDate { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int EventId { get; set; }
        public virtual Event Event { get; set; }
    }
}
