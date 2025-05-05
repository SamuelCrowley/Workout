using Microsoft.AspNetCore.Mvc;
using Workout.Application.Exceptions.Interfaces;

namespace Workout.Web.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected IActionResult APIError(IAPIException exception)
        {
            return StatusCode(exception.ExceptionNumber, new
            {
                success = false,
                exception.Message
            });
        }

        protected IActionResult CatchException(Exception ex)
        {
            if (ex is IAPIException)
            {
                IAPIException exception = (IAPIException)ex;
                return APIError(exception);
            }
            else
            {
                return StatusCode(500, new
                {
                    success = false,
                    ex.Message
                });
            }
        }
    }
}
