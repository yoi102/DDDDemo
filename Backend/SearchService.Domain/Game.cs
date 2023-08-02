using DomainCommons;

namespace SearchService.Domain;
public record Game(Guid Id, string TitleZh,string TitleEn,string TitleJa, Uri CoverUrl, string Introduction, string[]? Tags);