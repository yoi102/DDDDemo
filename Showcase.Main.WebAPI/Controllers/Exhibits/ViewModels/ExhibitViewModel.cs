using Showcase.Domain.Entities;

namespace Showcase.Main.WebAPI.Controllers.Exhibits.ViewModels
{
    public record ExhibitViewModel(ExhibitId Id, Uri ItemUrl)
    {
        public static ExhibitViewModel? Create(Exhibit? exhibit)
        {
            if (exhibit == null)
            {
                return null;
            }
            return new ExhibitViewModel(exhibit.Id, exhibit.ItemUrl);
        }

        public static ExhibitViewModel[] Create(Exhibit[] exhibit)
        {
            return exhibit.Select(e => Create(e)!).ToArray();
        }
    }
}