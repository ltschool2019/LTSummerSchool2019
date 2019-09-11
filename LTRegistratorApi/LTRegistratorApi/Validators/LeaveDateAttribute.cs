using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Validators
{
    public class LeaveDateAttribute : ValidationAttribute
    {
        private readonly string _validationWithDate;

        public LeaveDateAttribute()
        {
        }

        public LeaveDateAttribute(string validationWithDate)
        {
            _validationWithDate = validationWithDate;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime firstDate = (DateTime)value;
            if (string.IsNullOrEmpty(_validationWithDate))
            {
                var yearsDifference = firstDate.Year - DateTime.Today.Year;
                return 0 <= yearsDifference && yearsDifference <= 1 ? ValidationResult.Success : new ValidationResult("You can specify leaves only for the current and next year.");
            }

            DateTime secondDate = (DateTime)validationContext.ObjectType.GetProperty(_validationWithDate).GetValue(validationContext.ObjectInstance, null);

            return secondDate <= firstDate ? ValidationResult.Success : new ValidationResult("Date is not later");
        }
    }
}
