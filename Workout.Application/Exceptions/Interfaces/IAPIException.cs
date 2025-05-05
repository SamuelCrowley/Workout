namespace Workout.Application.Exceptions.Interfaces
{
    public interface IAPIException
    {
        public int ExceptionNumber { get; }
        public string Message { get; }
    }
}
