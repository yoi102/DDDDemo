using FluentValidation;
using Showcase.Admin.WebAPI.Controllers.Games.Requests;
using Showcase.Infrastructure;
using Infrastructure.EFCore;
using Showcase.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Showcase.Admin.WebAPI.Controllers.Games.Validators
{
    public class GameUpdateRequestValidator : AbstractValidator<GameUpdateRequest>
    {
        public GameUpdateRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Title.Chinese).NotNull().Length(1, 200);
            RuleFor(x => x.Title.English).NotNull().Length(1, 200);
            RuleFor(x => x.Title.Japanese).NotNull().Length(1, 200);

        }
    }

}
