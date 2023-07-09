using GameManagement.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.ValidationAttributes
{
    public class NameMustDifferentFromIntroductionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var addDto = (CompanyAddDto)validationContext.ObjectInstance;

            return addDto.Introduction == addDto.Name
                ? new ValidationResult(ErrorMessage, new[] { nameof(CompanyAddDto) })
                : ValidationResult.Success;
        }
    }
}
