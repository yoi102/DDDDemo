using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement.Shared.Entities
{
    public class Tag : EntityBase
    {
        [Required]
        public string Content { get; set; } = string.Empty;

        public ICollection<Game> Games { get; set; } = new List<Game>();

    }
}
