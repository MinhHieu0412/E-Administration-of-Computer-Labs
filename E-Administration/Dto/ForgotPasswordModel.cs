using System.ComponentModel.DataAnnotations;

namespace E_Administration.Dto
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
