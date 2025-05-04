using Microsoft.AspNetCore.Mvc;

namespace Workout.Web.Exceptions
{
    public abstract class APIException : Exception
    {
        public abstract int ExceptionNumber { get; }

        public APIException(string message) : base(message) { }

        public Func<IActionResult> GetError { get; set; }
        public APIException(Func<IActionResult> getError)
        {
            GetError = getError;
        }
    }

    public class BadRequestException : APIException
    {
        public override int ExceptionNumber { get => 400; }

        public BadRequestException(Func<IActionResult> getError) : base(getError)
        {

        }
    }
}
