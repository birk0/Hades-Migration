using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;


namespace HadesWeb.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = " ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Invalid Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class UsersList
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}