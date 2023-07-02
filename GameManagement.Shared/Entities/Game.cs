using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement.Shared.Entities
{
    public class Game : EntityBase
    {
        public int EmployeeNo { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Subtitle { get; set; } = string.Empty;
        [Url]
        public string CoverUrl { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }


        public ICollection<ImageUrl> ImageUrl { get; set; } = new List<ImageUrl>();

        public Publisher? Publisher { get; set; }
        public Guid? PublisherId { get; set; }
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();




    }
}
