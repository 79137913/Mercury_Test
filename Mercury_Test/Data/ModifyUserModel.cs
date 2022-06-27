using System.ComponentModel.DataAnnotations;

namespace Mercury_Test.Data
{
    public class ModifyUserModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        //Gender: 0 dont want to respond , 1 male, 2 female, next numbers reserved for further genders
        public int? Gender { get; set; }
    }
}
