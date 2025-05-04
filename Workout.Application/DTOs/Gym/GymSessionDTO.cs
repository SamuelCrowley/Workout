using Workout.Application.DTOs.Base;
using Workout.Domain.Data.Interfaces.Gym;
using Workout.Domain.Enums;

namespace Workout.Application.DTOs.Gym
{
    public class GymSessionDTO : ChildDTO
    {
        public override EntityObjectType EOType() => EntityObjectType.GymSessionEO;

        public string GymSessionType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public List<GymExerciseDTO> GymExerciseDTOs { get; set; }
        public int DurationMinutes => EndTime.HasValue ? (int)Math.Round((EndTime.Value - StartTime).TotalMinutes) : 0;

        public int ExerciseCount => GymExerciseDTOs?.Count ?? 0;

        public int TotalSets => GymExerciseDTOs?.Sum(e => e.GymSetDTOs?.Count ?? 0) ?? 0;

        public GymSessionDTO(IGymSessionData gymSessionEO) : base(gymSessionEO)
        {
            GymSessionType = gymSessionEO.GymSessionType;
            StartTime = gymSessionEO.StartTime;
            EndTime = gymSessionEO.EndTime;

            GymExerciseDTOs = new List<GymExerciseDTO>(
                gymSessionEO.GymExerciseEOs.Select(x => new GymExerciseDTO(x))
            );
        }
    }
}
