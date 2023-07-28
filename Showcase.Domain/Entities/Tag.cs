using DomainCommons;
using Strongly;

namespace Showcase.Domain.Entities
{
    public record Tag : BaseEntity
    {
        public Tag(TagId id, string text)
        {
            Id = id;
            Text = text;
        }

        public TagId Id { get; private set; }

        public string Text { get; private set; }

        public Tag ChangeText(string value)
        {
            Text = value;
            return this;
        }


    }
    [Strongly(converters: StronglyConverter.EfValueConverter | StronglyConverter.SwaggerSchemaFilter)]
    public partial struct TagId { }

}
