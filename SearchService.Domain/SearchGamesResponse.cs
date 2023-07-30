namespace SearchService.Domain;
public record SearchGamesResponse(IEnumerable<Game> Games, long TotalCount);
