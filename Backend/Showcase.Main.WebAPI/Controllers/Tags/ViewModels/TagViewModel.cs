using Showcase.Domain.Entities;

namespace Showcase.Main.WebAPI.Controllers.Tags.ViewModels
{
    public record TagViewModel(TagId Id, string Text)
    {
        public static TagViewModel? Create(Tag? tag)
        {
            if (tag == null)
            {
                return null;
            }
            return new TagViewModel(tag.Id, tag.Text);
        }

        public static TagViewModel[] Create(Tag[] tags)
        {
            return tags.Select(t => Create(t)!).ToArray();
        }
    }
}