using DomainCommons;
using Strongly;

namespace Showcase.Domain.Entities
{
    public record Game : AggregateRootEntity, IAggregateRoot
    {
        public Game(CompanyId companyId, GameId id, MultilingualString title, string introduction, Uri coverUrl, DateTimeOffset releaseDate, int sequenceNumber)
        {
            CompanyId = companyId;
            Id = id;
            Title = title;
            Introduction = introduction;
            CoverUrl = coverUrl;
            ReleaseDate = releaseDate;
            SequenceNumber = sequenceNumber;
        }

        public GameId Id { get; private set; }
        public CompanyId CompanyId { get; private set; }

        public MultilingualString Title { get; private set; }
        public string Introduction { get; private set; }
        public Uri CoverUrl { get; private set; }
        public DateTimeOffset ReleaseDate { get; private set; }
        public int SequenceNumber { get; private set; }

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

    [Strongly]
    public partial struct GameId
    { }
}