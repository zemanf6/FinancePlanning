using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Presentation.Areas.Auth.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
