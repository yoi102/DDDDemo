using FluentValidation;
using SearchService.WebAPI.Controllers.Requests;

namespace SearchService.WebAPI.Controllers.Validators
{
    public class SearchGamesRequestValidator : AbstractValidator<SearchGamesRequest>
    {
        public SearchGamesRequestValidator()
        {
            RuleFor(e => e.Keyword).NotNull().MinimumLength(0).MaximumLength(100);
            RuleFor(e => e.PageIndex).GreaterThan(0);//页号从1开始
            RuleFor(e => e.PageSize).GreaterThanOrEqualTo(5);
        }
    }
}