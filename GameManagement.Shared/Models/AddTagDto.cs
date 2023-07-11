using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.Models
{
    public class AddTagDto
    {
        [Required]
        public string? Tag { get; set; }
        public GameAddDto? Game { get; set; }
    }
}
