using E_Administration.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_Administration.Dto
{
    public class AssignmentDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int LabID { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [JsonConverter(typeof(Utilities.TimeSpanConverter))]
        public TimeSpan TimeStart { get; set; }

        [Required]
        [JsonConverter(typeof(Utilities.TimeSpanConverter))]
        public TimeSpan TimeEnd { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }
    }
}
