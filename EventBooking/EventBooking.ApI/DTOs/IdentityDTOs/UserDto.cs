namespace EventBooking.ApI.DTOs.IdentityDTOs
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string DisplayName { get; set; }
        public string? Image { get; set; }
    }
}
