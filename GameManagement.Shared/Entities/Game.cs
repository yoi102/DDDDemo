using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.Entities
{
    public class Game : EntityBase
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;
        [StringLength(50, MinimumLength = 1)]
        public string Subtitle { get; set; } = string.Empty;
        [Url]
        public string CoverUrl { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }


        public ICollection<DisplayItem> DisplayItems { get; set; } = new List<DisplayItem>();

        public Company? Company { get; set; }
        public Guid? CompanyId { get; set; }
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();




    }
}
