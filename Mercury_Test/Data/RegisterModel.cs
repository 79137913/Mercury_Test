using System.ComponentModel.DataAnnotations;

namespace Mercury_Test.Data
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public DateTime? Birthday { get; set; }
        //Gender: 0 dont want to respond , 1 male, 2 female, next numbers reserved for further genders
        public int? Gender { get; set; }
    }
}
