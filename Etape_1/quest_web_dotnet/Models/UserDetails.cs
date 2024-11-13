using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace quest_web.Models
{
    public enum UserRole
    {
        ROLE_USER = 0,
        ROLE_ADMIN = 1
    }

    public class UserDetails
    {
        public int ID { get; set; }

        public string Username { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }
    }
}