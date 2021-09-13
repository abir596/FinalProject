using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You need to give us your Last Name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You need to give us your Date of Birth")]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "You are required to indicate your Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "You need to give us your Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Confirm Email")]
        [Required(ErrorMessage = "You need to Confirm your Email Address")]
        [DataType(DataType.EmailAddress)]
        [Compare("Email", ErrorMessage = "Your Email Addresses do not match")]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessage = "You are required to select your Country")]
        public string Country { get; set; }

        [Display(Name = "Create Password")]
        [Required(ErrorMessage = "You are required to create a Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$", ErrorMessage = "Password must be atleast 8 characters and contain one uppercase letter, one lowercase letter, one digit and one special character")]
        public string CreatePassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "You need to confirm your Password")]
        [DataType(DataType.Password)]
        [Compare("CreatePassword", ErrorMessage = "Your Passwords do not match")]
        public string ConfirmPassword { get; set; }
        public IList<Post> Posts { get; set; }
        public IList<ImageModel> Images { get; set; }
        public IList<VideoFiles> VideoFiles { get; set; }
    }
}
