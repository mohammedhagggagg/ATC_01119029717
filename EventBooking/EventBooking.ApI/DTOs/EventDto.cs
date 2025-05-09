using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventBooking.ApI.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        public List<EventPhotoDto>? Photos { get; set; } = new List<EventPhotoDto>();
        //public List<string>? ImageUrls { get; set; } = new List<string>();
        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }
}
