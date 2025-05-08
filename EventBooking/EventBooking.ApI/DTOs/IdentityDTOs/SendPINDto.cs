using System.ComponentModel.DataAnnotations;

namespace EventBooking.ApI.DTOs.IdentityDTOs
{
    public class SendPINDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
