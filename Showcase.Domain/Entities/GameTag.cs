namespace Showcase.Domain.Entities
{
    public class GameTag
    {
        public GameTag(Game Game, Tag Tag)
        {
            this.Game = Game;
            this.Tag = Tag;
        }
        public GameId GameId { get; set; }
        public Game Game { get; set; }
        public TagId TagId { get; set; }
        public Tag Tag { get; set; }

    }
}
