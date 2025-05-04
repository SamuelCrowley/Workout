using Workout.Domain.Data.Base.Interfaces;

namespace Workout.Domain.Data.Interfaces.Gym
{
    public interface IGymSessionData : IBaseDataStructure, IChildDataStructure
    {
        public ICollection<IGymExerciseData> GymExerciseEOs { get; }
        public IGymUserData GymUserEO { get; }

        public string GymSessionType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
