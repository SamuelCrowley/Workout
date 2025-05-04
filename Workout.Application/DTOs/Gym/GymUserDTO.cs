using Workout.Application.DTOs.Base;
using Workout.Domain.Data.Interfaces.Gym;
using Workout.Domain.Enums;

namespace Workout.Application.DTOs.Gym
{
    public class GymUserDTO : ChildDTO
    {
        public override EntityObjectType EOType()
        {
            return EntityObjectType.GymUserEO;
        }

        public List<GymSessionDTO> GymSessionDTOs { get; set; }

        public GymUserDTO(IGymUserData gymUserEO) : base(gymUserEO)
        {
            GymSessionDTOs = new List<GymSessionDTO>(gymUserEO.GymSessionEOs.Select(x => new GymSessionDTO(x)));
        }
    }
}
