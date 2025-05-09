namespace EventBooking.ApI.DTOs
{
    public class EventCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; } 
        public List<IFormFile>? Photos { get; set; } = new List<IFormFile>();
    }
}
