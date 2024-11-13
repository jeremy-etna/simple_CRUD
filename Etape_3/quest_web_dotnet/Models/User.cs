using System.ComponentModel.DataAnnotations.Schema;

namespace quest_web.Models
{
    public class User : UserDetails
    {
        [Column("password", TypeName = "varchar(255)")]
        public string Password { get; set; }

        [Column("creation_date", TypeName = "datetime")]
        public DateTime Creation_Date { get; set; }

        [Column("updated_date", TypeName = "datetime")]
        public DateTime Updated_Date { get; set; }

        public object GetUserDetails()
        {
            return new 
            {
                username =  this.Username ,
                role = this.Role.ToString()
            };
        }
    }
}