using GameManagement.Shared.Entities;
using GameManagement.Shared.ValidationAttributes;
using System.ComponentModel.DataAnnotations;


namespace GameManagement.Shared.Models
{
    [TitleNoMustDifferentFromIntroduction(ErrorMessage = "标题必须和简介不一样！！")]
    public abstract class GameAddOrUpdateDto : IValidatableObject
    {


        [Display(Name = "标题")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "{0}的长度是{1}")]
        public string Title { get; set; } = string.Empty;
        [Display(Name = "副标题")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "{0}的长度是{1}")]
        public string Subtitle { get; set; } = string.Empty;
        [Url]
        public string CoverUrl { get; set; } = string.Empty;
        [Display(Name = "简介")]
        public string Introduction { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }


        public ICollection<string> ImageUrl { get; set; } = new List<string>();





        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == Subtitle)
            {
                yield return new ValidationResult("标题和副标题不能一样",
                    new[] { nameof(Title), nameof(Subtitle) });
            }
        }
    }
}
