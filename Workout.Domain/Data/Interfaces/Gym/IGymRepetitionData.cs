using Workout.Domain.Data.Base.Interfaces;
using Workout.Domain.Enums;

namespace Workout.Domain.Data.Interfaces.Gym
{
    public interface IGymRepetitionData : IBaseDataStructure, IChildDataStructure
    {
        public IGymSetData GymSetEO { get; }

        public float Weight { get; set; }
        public RepetitionDifficulty Difficulty { get; set; }

        public int Order { get; set; }

    }
}
