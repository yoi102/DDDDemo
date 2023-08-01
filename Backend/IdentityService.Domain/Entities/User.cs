using DomainCommons;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

public class User : IdentityUser<Guid>, IHasCreationTime, IHasDeletionTime, ISoftDelete
{
    public DateTimeOffset CreationTime { get; init; }

    public DateTimeOffset? DeletionTime { get; private set; }

    public bool IsDeleted { get; private set; }

    public User(string userName) : base(userName)
    {
        Id = Guid.NewGuid();
        CreationTime = DateTimeOffset.Now;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletionTime = DateTimeOffset.Now;
    }
}