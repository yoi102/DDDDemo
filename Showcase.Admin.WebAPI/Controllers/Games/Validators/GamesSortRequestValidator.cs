using FluentValidation;
using Showcase.Admin.WebAPI.Controllers.Games.Requests;

namespace Showcase.Admin.WebAPI.Controllers.Games.Validators
{
    public class GamesSortRequestValidator : AbstractValidator<GamesSortRequest>
    {
        public GamesSortRequestValidator()
        {
            RuleFor(r => r.SortedGameIds).NotNull().NotEmpty().NotContains(new Domain.Entities.GameId(Guid.Empty)).NotDuplicated();
            //RuleFor(r => r.SortedGameIds).NotNull().NotEmpty().NotContains(Guid.Empty).NotDuplicated();
        }
    }
}
