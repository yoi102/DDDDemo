using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.Entities
{
    public class Tag : EntityBase
    {
        [Required]
        public string Content { get; set; } = string.Empty;

        public ICollection<Game> Games { get; set; } = new List<Game>();

    }
}
