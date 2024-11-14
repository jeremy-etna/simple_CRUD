using System.ComponentModel.DataAnnotations;

namespace quest_web.DTO
{
    public class UserAnthenticationParams
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserRegistrationParams
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int Role { get; set; }
    }

    public class UserDetailsResponse
    {
        public string username { get; set; }
        public int role { get; set; }
        public int id { get; set; }
    }

    public class UserAuthenticationResponse
    {
        public string token { get; set; }
    }

    public class UserNameUpdateParams
    {
        public string? Username { get; set; }
        public int? Role { get; set; }
    }
}