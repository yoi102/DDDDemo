using DomainCommons;
using Showcase.Domain.Events;
using Strongly;

namespace Showcase.Domain.Entities
{
    public record Game : AggregateRootEntity, IAggregateRoot
    {
        private Game()
        {
        }

        public static Game Create(CompanyId companyId, GameId id, MultilingualString title, string introduction, Uri coverUrl, DateTimeOffset releaseDate, int sequenceNumber)
        {
            var g = new Game()
            {
                CompanyId = companyId,
                Id = id,
                Title = title,
                Introduction = introduction,
                CoverUrl = coverUrl,
                ReleaseDate = releaseDate,
                SequenceNumber = sequenceNumber,
            };
            g.AddDomainEvent(new GameCreatedEvent(g));//ef core...
            return g;
        }

        public GameId Id { get; private set; }
        public CompanyId CompanyId { get; private set; }
        public ICollection<TagId> TagIds { get; private set; } = new HashSet<TagId>();
        public MultilingualString Title { get; private set; }
        public string Introduction { get; private set; }
        public Uri CoverUrl { get; private set; }
        public DateTimeOffset ReleaseDate { get; private set; }
        public int SequenceNumber { get; private set; }

        public Game AddTag(TagId tagId)
        {
            TagIds.Add(tagId);
            return this;
        }
        public Game RemoveTag(TagId tagId)
        {
            TagIds.Remove(tagId);
            return this;
        }

        public Game ChangeTitle(MultilingualString value)
        {
            Title = value;
            return this;
        }
        public Game ChangeIntroduction(string value)
        {
            Introduction = value;
            return this;
        }

        public Game ChangeCoverUrl(Uri value)
        {
            CoverUrl = value;
            return this;
        }
        public Game ChangeReleaseDate(DateTimeOffset value)
        {
            ReleaseDate = value;
            return this;
        }

        public Game ChangeSequenceNumber(int value)
        {
            SequenceNumber = value;
            return this;
        }
    }

    [Strongly(converters: StronglyConverter.EfValueConverter | StronglyConverter.SwaggerSchemaFilter)]
    public partial struct GameId
    { }
}