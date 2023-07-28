using DomainCommons;
using Strongly;

namespace Showcase.Domain.Entities
{
    public record Company : AggregateRootEntity, IAggregateRoot
    {
        public Company(CompanyId id, string name, Uri coverUrl, int sequenceNumber)
        {
            Id = id;
            Name = name;
            CoverUrl = coverUrl;
            SequenceNumber = sequenceNumber;

        }

        public CompanyId Id { get; private set; }
        public string Name { get; private set; }
        public Uri CoverUrl { get; private set; }
        public int SequenceNumber { get; private set; }

        public Company ChangeSequenceNumber(int value)
        {
            this.SequenceNumber = value;
            return this;
        }
        public Company ChangeName(string value)
        {
            this.Name = value;
            return this;
        }

        public Company ChangeCoverUrl(Uri value)
        {
            this.CoverUrl = value;
            return this;
        }
    }

    [Strongly(converters: StronglyConverter.EfValueConverter| StronglyConverter.SwaggerSchemaFilter)]
    public partial struct CompanyId
    {
    }
}