using System;

namespace quest_web.Models
{
    public class User : UserDetails
    {

        public string Password { get; set; }
        public DateTime Creation_Date { get; set; }
        public DateTime Updated_Date { get; set; }
    }
}