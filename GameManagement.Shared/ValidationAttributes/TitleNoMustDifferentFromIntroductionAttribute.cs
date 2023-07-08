using GameManagement.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.ValidationAttributes
{
    public class TitleNoMustDifferentFromIntroductionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var addDto = (GameAddOrUpdateDto)validationContext.ObjectInstance;

            return addDto.Introduction == addDto.Title
                ? new ValidationResult(ErrorMessage, new[] { nameof(GameAddOrUpdateDto) })
                : ValidationResult.Success;
        }
    }
}
