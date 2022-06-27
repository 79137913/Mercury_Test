using Microsoft.AspNetCore.Identity;

namespace Mercury_Test.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        //Gender: 0 dont want to respond , 1 male, 2 female, next numbers reserved for further genders
        public int? Gender { get; set; }
    }
}
