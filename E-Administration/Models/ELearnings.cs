using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Administration.Models
{
    public class Elearnings
    {
        public int ID { get; set; }

        [Required]
        public int UploadedBy { get; set; } // UserID of the uploader

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } // Title of the e-learning content

        public string Description { get; set; } // Optional description

        [MaxLength(255)]
        public string FilePath { get; set; } // Optional file path for uploaded documents

        [MaxLength(255)]
        public string Link { get; set; } // Optional link for online resources

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedAt { get; set; } // Automatically updated on changes

        // Navigation properties
        [ForeignKey("UploadedBy")]
        public virtual User User { get; set; } // Relation to Users table
    }

}
