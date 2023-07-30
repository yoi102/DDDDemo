

using DomainCommons;

namespace SearchService.Domain;
public record Game(Guid Id, MultilingualString Title, Uri CoverUrl, string Introduction, Guid[] TagIds);
