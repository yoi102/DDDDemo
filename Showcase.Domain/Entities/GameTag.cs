using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showcase.Domain.Entities
{
    public class GameTag
    {
        public GameId GameId { get; set; }
        public TagId TagId { get; set; }

    }
}
