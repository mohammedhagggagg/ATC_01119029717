using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBooking.DAL.Models
{
    public class EventPhoto :BaseEntity
    {
        public string PhotoLink { get; set; }
        // Foreign key
        public int EventId { get; set; }
        public Event Event { get; set; }

    }
}
