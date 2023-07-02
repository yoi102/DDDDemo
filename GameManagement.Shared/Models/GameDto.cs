using System;

namespace GameManagement.Shared.Models
{
    public class GameDto
    {

        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }

        public ICollection<string> ImageUrl { get; set; } = new List<string>();


        public ICollection<string> Tags { get; set; } = new List<string>();
    }
}
