using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBooking.DAL.Models
{
    public class Event :BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; } // optional
        public DateTime Date { get; set; }
        public string Venue { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        // Foreign key
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}
