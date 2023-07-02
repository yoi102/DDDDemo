using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.Entities
{
    public class ImageUrl : EntityBase
    {
        [Url]
        [Required]
        public string Url { get; set; } = string.Empty;

        public Guid GameId { get; set; }
        public Game Game { get; set; } = new Game();

    }
}