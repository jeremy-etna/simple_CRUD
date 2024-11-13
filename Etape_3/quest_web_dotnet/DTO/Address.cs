using System.ComponentModel.DataAnnotations;

namespace quest_web.DTO
{
    public class AddressCreationParams
    {
        [Required]
        public string road { get; set; }
        [Required]
        public string postalCode { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public string country { get; set; }
    }

    public class AddressUpdateParams
    {
        public string? road { get; set; }
        public string? postalCode { get; set; }
        public string? city { get; set; }
        public string? country { get; set; }
    }
}

