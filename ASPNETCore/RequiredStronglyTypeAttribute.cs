using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore
{


    public class RequiredStronglyTypeAttribute : ValidationAttribute
    {
        public const string DefaultErrorMessage = "The {0} field must be strongly type and the value not Guid.Empty";

        public RequiredStronglyTypeAttribute()
            : base("The {0} field must be strongly type and the value not Guid.Empty")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return false;
            }

            var type = value.GetType();

            if (!type.IsValueType)
            {
                return false;
            }
            var info = type.GetProperty("Value");

            if (info is null)
            {
                return false;
            }
            var obj = info.GetValue(value);

            if (obj is Guid)
            {
                return (Guid)obj != Guid.Empty;
            }

            return false;
        }
    }





}
