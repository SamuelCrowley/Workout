using Workout.Domain.Data.Base.Interfaces;

namespace Workout.Domain.Data.Interfaces.Gym
{
    public interface IGymExerciseData : IBaseDataStructure, IChildDataStructure
    {
        public ICollection<IGymSetData> GymSetEOs { get; }
        public IGymSessionData GymSessionEO { get; }

        public string ExerciseName { get; set; }
        public DateTime StartTime { get; set; }
    }
}
