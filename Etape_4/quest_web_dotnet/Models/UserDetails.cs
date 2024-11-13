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
        [Column("id", TypeName = "int(11)")]
        public int ID { get; set; }
        [Column("username", TypeName = "varchar(255)")]
        public string Username { get; set; }

        [Column("role", TypeName = "varchar(255)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }
    }
}