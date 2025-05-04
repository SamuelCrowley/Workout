using Workout.Application.DTOs.Base;
using Workout.Domain.Data.Interfaces.Gym;
using Workout.Domain.Enums;

namespace Workout.Application.DTOs.Gym
{
    public class GymRepetitionDTO : ChildDTO
    {
        public override EntityObjectType EOType()
        {
            return EntityObjectType.GymRepetitionEO;
        }

        public float Weight { get; set; }
        public RepetitionDifficulty Difficulty { get; set; }

        public int Order { get; set; }

        public GymRepetitionDTO(IGymRepetitionData gymRepetitionEO) : base(gymRepetitionEO)
        {
            Weight = gymRepetitionEO.Weight;
            Difficulty = gymRepetitionEO.Difficulty;
            Order = gymRepetitionEO.Order;
        }
    }
}
