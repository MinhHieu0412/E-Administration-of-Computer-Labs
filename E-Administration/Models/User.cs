using System.ComponentModel.DataAnnotations;

namespace E_Administration.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Image { get; set; }
        public string? Role { get; set; }

    }
}
