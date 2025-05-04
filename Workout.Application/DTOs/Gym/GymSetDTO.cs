using Workout.Application.DTOs.Base;
using Workout.Domain.Data.Interfaces.Gym;
using Workout.Domain.Enums;

namespace Workout.Application.DTOs.Gym
{
    public class GymSetDTO : ChildDTO
    {
        public override EntityObjectType EOType()
        {
            return EntityObjectType.GymSetEO;
        }

        public DateTime StartTime { get; set; }

        public List<GymRepetitionDTO> GymRepetitionDTOs { get; set; }

        public GymSetDTO(IGymSetData gymSetEO) : base(gymSetEO)
        {
            StartTime = gymSetEO.StartTime;

            GymRepetitionDTOs = new List<GymRepetitionDTO>(gymSetEO.GymRepetitionEOs.Select(x => new GymRepetitionDTO(x)));
        }
    }
}
