using System.ComponentModel.DataAnnotations;

namespace E_Administration.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Description { get; set; }
    }
}
