using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.Entities
{
    public class Publisher : EntityBase
    {
        [Required]
        [StringLength(maximumLength: 50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        public string Introduction { get; set; } = string.Empty;
        [Required]
        public DateTimeOffset EstablishmentTime { get; set; }
        public DateTimeOffset? BankruptTime { get; set; }

        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}