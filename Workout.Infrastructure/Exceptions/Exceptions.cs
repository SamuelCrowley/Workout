using Microsoft.AspNetCore.Mvc;

namespace Workout.Infrastructure.Exceptions
{
    /// <summary>
    /// SEC 05-May-2025 
    /// This should really be changed into an interface, as Web shouldn't directly use code from Infrastructure unless it's wiring dependencies.
    /// The reason for this being done the current way is a design oversight, and should be changed at the first opportunity.
    /// </summary>
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
