using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventBooking.ApI.DTOs
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; }

       
        [MaxLength(40)]
        public string? Name { get; set; }

        [MaxLength(80)]
       
        public string? Description { get; set; }

        [JsonIgnore]  //Hide Response 
        public IFormFile? Photo { get; set; }
        public string? PhotoName { get; set; }
    }
}
