using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Workout.Application.Exceptions.Interfaces;

namespace Workout.Web.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception is IAPIException apiEx)
            {
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = apiEx.Message
                })
                {
                    StatusCode = apiEx.ExceptionNumber
                };
            }
            else
            {
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = "An error occurred",
                    error = exception.Message
                })
                {
                    StatusCode = 500
                };
            }

            context.ExceptionHandled = true;
        }
    }
}
