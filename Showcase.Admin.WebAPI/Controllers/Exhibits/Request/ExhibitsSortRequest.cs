using Showcase.Domain.Entities;

namespace Showcase.Admin.WebAPI.Controllers.Exhibits.Request
{
    public class ExhibitsSortRequest
    {
        public required ExhibitId[] SortedExhibitIds { get; set; }

    }
}
