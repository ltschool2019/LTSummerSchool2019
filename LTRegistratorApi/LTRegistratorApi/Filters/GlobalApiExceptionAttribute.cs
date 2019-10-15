using System;
using System.Linq;
using System.Net;
using LTRegistrator.BLL.Contracts.Exceptions;
using LTRegistratorApi.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LTRegistratorApi.Filters
{
    public class GlobalApiExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            HttpStatusCode statusCode;
            if (exception is NotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else if (exception is ForbiddenException)
            {
                statusCode = HttpStatusCode.Forbidden;
            }
            else if (exception is ConflictException)
            {
                statusCode = HttpStatusCode.Conflict;
            }
            else if (exception is BadRequestException)
            {
                statusCode = HttpStatusCode.BadRequest;
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            context.HttpContext.Response.StatusCode = (int)statusCode;
            var message = statusCode == HttpStatusCode.InternalServerError
                ? "Internal Server Error"
                : exception.Message;

            var apiError = new ApiError(message);

            context.Result = new JsonResult(apiError);
        }
    }
}
