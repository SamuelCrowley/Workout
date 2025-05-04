using Workout.Domain.Data.Base.Interfaces;

namespace Workout.Domain.Data.Interfaces.Gym
{
    public interface IGymSetData : IBaseDataStructure, IChildDataStructure
    {
        public ICollection<IGymRepetitionData> GymRepetitionEOs { get; }
        public IGymExerciseData GymExerciseEO { get; }

        public DateTime StartTime { get; set; }
    }
}
