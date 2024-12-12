using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Models
{
    public class AboutUs
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? Mission { get; set; }
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
