using Workout.Application.DTOs.Base;
using Workout.Domain.Data.Interfaces.Gym;
using Workout.Domain.Enums;

namespace Workout.Application.DTOs.Gym
{
    public class GymExerciseDTO : ChildDTO
    {
        public override EntityObjectType EOType()
        {
            return EntityObjectType.GymExerciseEO;
        }

        public string ExerciseName { get; set; }
        public DateTime StartTime { get; set; }

        public List<GymSetDTO> GymSetDTOs { get; set; }

        public GymExerciseDTO(IGymExerciseData gymExerciseEO) : base(gymExerciseEO)
        {
            StartTime = gymExerciseEO.StartTime;
            ExerciseName = gymExerciseEO.ExerciseName;

            GymSetDTOs = new List<GymSetDTO>(gymExerciseEO.GymSetEOs.Select(x => new GymSetDTO(x)));
        }
    }
}
