using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Infrastructure.EFCore;
using Showcase.Admin.WebAPI.Controllers.Exhibits.Request;
using Showcase.Domain.Entities;
using Showcase.Infrastructure;

namespace Showcase.Admin.WebAPI.Controllers.Exhibits.Validators
{
    public class ExhibitAddRequestValidator : AbstractValidator<ExhibitAddRequest>
    {
        public ExhibitAddRequestValidator(ShowcaseDbContext dbContext)
        {
            RuleFor(x => x.ItemUrl).NotEmpty().Length(5, 500);
            RuleFor(x => x.GameId).Must((cId, ct) => dbContext.Query<Game>().Any(c => c.Id == cId.GameId))
           .WithMessage(c => $" GameId={c.GameId} 不存在");
        }
    }
}