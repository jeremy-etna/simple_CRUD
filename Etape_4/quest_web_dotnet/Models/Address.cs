using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;

namespace quest_web.Models
{
    public class Address
    {
        [Column("id", TypeName = "int(11)")]
        public int id { get; set; }

        [Column("street", TypeName = "varchar(100)")]
        public string street { get; set; }

        [Column("postal_code", TypeName = "varchar(30)")]
        public string postalCode { get; set; }

        [Column("city", TypeName = "varchar(50)")]
        public string city { get; set; }

        [Column("country", TypeName = "varchar(50)")]
        public string country { get; set; }

        [Column("user_id", TypeName = "int(11)")]
        public int User { get; set; }

        [Column("creation_date", TypeName = "datetime")]
        public DateTime? creationDate { get; set; }

        [Column("updated_date", TypeName = "datetime")]
        public DateTime? updatedDate { get; set; }
    }
}