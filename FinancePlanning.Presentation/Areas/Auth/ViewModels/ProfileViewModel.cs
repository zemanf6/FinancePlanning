using FinancePlanning.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Presentation.Areas.Auth.ViewModels
{
    public class ProfileViewModel
    {
        public string Email { get; set; } = string.Empty;
    }
}
