using FluentValidation;
using Infrastructure.EFCore;
using Showcase.Admin.WebAPI.Controllers.Tags.Requests;
using Showcase.Domain.Entities;
using Showcase.Infrastructure;

namespace Showcase.Admin.WebAPI.Controllers.Tags.Validators
{
    public class TagAddRequestValidator : AbstractValidator<TagAddRequest>
    {
        public TagAddRequestValidator(ShowcaseDbContext dbContext)
        {
            RuleFor(x => x.Text).NotEmpty().Must((cId, ct) => dbContext.Query<Tag>().Any(c => c.Text == cId.Text))
                .WithMessage(c => $" Text={c.Text} 已经存在"); ;
            RuleFor(x => x.GameId).Must((cId, ct) => dbContext.Query<Game>().Any(c => c.Id == cId.GameId))
          .WithMessage(c => $" GameId={c.GameId} 不存在");
        }
    }
}