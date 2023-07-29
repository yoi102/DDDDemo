using FluentValidation;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Showcase.Admin.WebAPI.Controllers.Games.Requests;
using Showcase.Domain.Entities;
using Showcase.Infrastructure;

namespace Showcase.Admin.WebAPI.Controllers.Games.Validators
{
    public class GameAddRequestValidator : AbstractValidator<GameAddRequest>
    {
        public GameAddRequestValidator(ShowcaseDbContext dbContext)
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Title.Chinese).NotNull().Length(1, 200);
            RuleFor(x => x.Title.English).NotNull().Length(1, 200);
            RuleFor(x => x.Title.Japanese).NotNull().Length(1, 200);
            RuleFor(x => x.CoverUrl).Length(5, 500);//CoverUrl允许为空
            RuleFor(x => x.CompanyId).MustAsync((cId, ct) => dbContext.Query<Game>().AnyAsync(c => c.Id.Value == cId.Value))
                .WithMessage(c => $" CompanyId={c.CompanyId} 不存在");
        }
    }

}
