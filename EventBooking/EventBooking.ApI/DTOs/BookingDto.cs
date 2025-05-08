using System.ComponentModel.DataAnnotations;

namespace EventBooking.ApI.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public DateTime BookingDate { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Number of tickets must be at least 1")]
        public int NumberOfTickets { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
