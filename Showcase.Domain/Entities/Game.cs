﻿using DomainCommons;
using Showcase.Domain.Events;
using Strongly;

namespace Showcase.Domain.Entities
{
    public record Game : AggregateRootEntity, IAggregateRoot
    {
        private Game(CompanyId companyId, GameId id, MultilingualString title, string introduction, Uri coverUrl, DateTimeOffset releaseDate, int sequenceNumber)
        {
            CompanyId = companyId;
            Id = id;
            Title = title;
            Introduction = introduction;
            CoverUrl = coverUrl;
            ReleaseDate = releaseDate;
            SequenceNumber = sequenceNumber;
            //AddDomainEvent(new GameCreatedEvent(this));//ef core...
        }

        public static Game Create(CompanyId companyId, GameId id, MultilingualString title, string introduction, Uri coverUrl, DateTimeOffset releaseDate, int sequenceNumber)
        {
            var g = new Game(companyId, id, title, introduction, coverUrl, releaseDate, sequenceNumber);
            g.AddDomainEvent(new GameCreatedEvent(g));//ef core...
            return g;
        }

        public GameId Id { get; private set; }
        public CompanyId CompanyId { get; private set; }
        //public ICollection<GameTag> GameTags { get; private set; } = new List<GameTag>();
        public MultilingualString Title { get; private set; }
        public string Introduction { get; private set; }
        public Uri CoverUrl { get; private set; }
        public DateTimeOffset ReleaseDate { get; private set; }
        public int SequenceNumber { get; private set; }


        //public void RemoveTag(TagId tagId)
        //{
        //    var gameTag = GameTags.SingleOrDefault(gt => gt.TagId == tagId && gt.GameId == Id);
        //    if (gameTag != null)
        //    {
        //        GameTags.Remove(gameTag);
        //    }
        //}
        //public void AddProduct(TagId tagId)
        //{
        //    // 建立分类和产品之间的关联
        //    var gameTag = new GameTag
        //    {
        //        TagId = tagId,
        //        GameId = Id
        //    };
        //    GameTags.Add(gameTag);
        //}


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