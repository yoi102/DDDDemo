using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.Models
{
    public class CompanyAddDto
    {
        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(100, ErrorMessage = "{0} maximum length not exceeded{1}")]
        public string? Name { get; set; }

        [Display(Name = "紹介")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "{0} string length range  is from {2} to {1}")]
        public string Introduction { get; set; } = string.Empty;

        public ICollection<GameAddDto> Games { get; set; } = new List<GameAddDto>();
    }
}
