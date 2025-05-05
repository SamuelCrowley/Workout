using Workout.Application.Exceptions.Interfaces;

namespace Workout.Infrastructure.Exceptions.Concrete
{
    public abstract class APIException : Exception, IAPIException
    {
        public abstract int ExceptionNumber { get; }

        public APIException(string message) : base(message) 
        {
        }
        string IAPIException.Message { get => Message; }
    }

    public class BadRequestException : APIException
    {
        public override int ExceptionNumber { get => 400; }

        public BadRequestException(string message) : base(message)
        {

        }
    }
}
