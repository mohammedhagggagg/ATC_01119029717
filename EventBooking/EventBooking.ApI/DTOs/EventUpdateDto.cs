using System.Text.Json;

namespace EventBooking.ApI.DTOs
{
    public class EventUpdateDto
    {
        //public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public List<IFormFile> PhotosToAdd { get; set; } 
        public List<int>? PhotoIdsToDelete { get; set; } = new List<int>();

    }
}
