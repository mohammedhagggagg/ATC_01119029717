using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventBooking.ApI.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(80)]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [JsonIgnore]  //Hide Response 
        public IFormFile? Photo { get; set; }
        public string? PhotoName { get; set; }
    }
}
