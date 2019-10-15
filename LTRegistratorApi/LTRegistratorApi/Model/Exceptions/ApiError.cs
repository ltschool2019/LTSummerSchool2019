using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LTRegistratorApi.Model.Exceptions
{
    public class ApiError
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        public string Details { get; set; }

        public ApiError(string message)
        {
            Message = message;
            IsError = true;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            IsError = true;
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                Message = "Please correct the specified errors and try again.";
            }
        }
    }
}
