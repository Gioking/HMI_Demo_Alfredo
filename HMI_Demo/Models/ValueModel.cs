using System.ComponentModel.DataAnnotations;

namespace HMI_Demo.Models
{
    public class ValueModel
    {
        [Required]
        public string? Base64String { get; set; }
        [Required]
        public string? DocName { get; set; }
    }
}
