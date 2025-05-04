using Workout.Domain.Data.Base.Interfaces;

namespace Workout.Domain.Data.Interfaces.Gym
{
    public interface IGymUserData : IBaseDataStructure, IChildDataStructure
    {
        public ICollection<IGymSessionData> GymSessionEOs { get; }
        public IApplicationUserEO ApplicationUserEO { get; }
    }
}
