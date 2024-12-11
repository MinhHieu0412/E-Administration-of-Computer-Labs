using System.ComponentModel.DataAnnotations;

namespace E_Administration.Models
{
    public class ELearning
    {
        [Key]
        public int Id { get; set; }
        public int UploadedBy { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        public string? Link { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set ; } = DateTime.Now;

    }
}
