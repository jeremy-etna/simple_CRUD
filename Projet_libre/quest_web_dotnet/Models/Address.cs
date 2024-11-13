using Microsoft.Extensions.Hosting;

namespace quest_web.Models
{
    public class Address
    {
        public int id { get; set; }
        public string road { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public int User { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime updatedDate { get; set; }
        public ICollection<Address> Adresses { get; } = new List<Address>();

        public bool IsValid()
        {
            return(road.Length != 0 && 
                   postalCode.Length != 0 && 
                   city.Length != 0 && 
                   country.Length != 0);
        }
    }
}