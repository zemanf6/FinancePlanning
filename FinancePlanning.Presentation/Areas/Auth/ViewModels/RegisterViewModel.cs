using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Presentation.Areas.Auth.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "I agree with the Privacy Policy")]
        public bool AcceptPrivacyPolicy { get; set; }
    }
}
