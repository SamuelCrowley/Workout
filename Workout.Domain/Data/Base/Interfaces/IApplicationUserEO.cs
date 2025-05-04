using Workout.Domain.Data.Interfaces.Gym;

namespace Workout.Domain.Data.Base.Interfaces
{
    public interface IApplicationUserEO
    {
        public string Id { get; set; }
        public IGymUserData GymUserEO { get; }
    }
}
