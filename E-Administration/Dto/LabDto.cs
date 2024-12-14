using E_Administration.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Dto
{
    public class LabDto
    {
        public int ID { get; set; }
        [Required]
        public string? Name { get; set; }
        public int DepartmentID { get; set; }

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }
        [Required]
        [StringLength(100)]
        public string? Location { get; set; }
        [Required]
        public int? Capacity { get; set; }
        [Required]
        public bool? IsOperational { get; set; } = true;

    }
}
