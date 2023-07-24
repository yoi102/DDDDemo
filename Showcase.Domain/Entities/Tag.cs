﻿using DomainCommons;
using Strongly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showcase.Domain.Entities
{
    public record Tag : AggregateRootEntity, IAggregateRoot
    {
        public Tag(GameId gameId, TagId id, string text)
        {
            GameId = gameId;
            Id = id;
            Text = text;
        }

        public TagId Id { get; private set; }
        public GameId GameId { get; private set; }
        public string Text { get; private set; }

        public Tag ChangeText(string value)
        {
            Text = value;
            return this;
        }


    }
    [Strongly]
    public partial struct TagId { }

}