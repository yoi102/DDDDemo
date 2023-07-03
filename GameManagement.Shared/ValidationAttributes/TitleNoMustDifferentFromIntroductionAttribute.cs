using GameManagement.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.ValidationAttributes
{
    public class TitleNoMustDifferentFromIntroductionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var addDto = (GameAddOrUpdateDto)validationContext.ObjectInstance;

            if (addDto.Introduction == addDto.Title)
            {
                return new ValidationResult(ErrorMessage, new[] { nameof(GameAddOrUpdateDto) });
            }

            return ValidationResult.Success;
        }
    }
}
