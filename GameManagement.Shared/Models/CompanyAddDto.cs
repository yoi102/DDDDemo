using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.Models
{
    public class CompanyAddDto
    {
        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(100, ErrorMessage = "The maximum length of {0} cannot exceed {1}")]
        public string? Name { get; set; }

        [Display(Name = "紹介")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "The length range of {0} is from {2} to {1}")]
        public string Introduction { get; set; } = string.Empty;

        [Required]
        public string? Country { get; set; }
        [Required]
        public DateTimeOffset EstablishmentTime { get; set; }

        public ICollection<GameAddDto> Games { get; set; } = new List<GameAddDto>();
    }
}
