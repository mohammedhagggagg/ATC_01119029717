using System.ComponentModel.DataAnnotations;

namespace EventBooking.ApI.DTOs.IdentityDTOs
{
    public class VerfiyPINDto
    {
        [Required]
        [Range(1, 999999, ErrorMessage = "Pin Code Must be 6 digits")]
        public int pin { get; set; }
    }
}
