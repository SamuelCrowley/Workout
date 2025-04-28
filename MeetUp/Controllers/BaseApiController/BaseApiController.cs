using Microsoft.AspNetCore.Mvc;

namespace MeetUp.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected IActionResult InternalServerError(string message, Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message,
                error = ex.Message
            });
        }

        protected IActionResult BadRequest(string message)
        { 
            return StatusCode(400, new
            {
                success = false,
                message
            });
        }
    }
}
