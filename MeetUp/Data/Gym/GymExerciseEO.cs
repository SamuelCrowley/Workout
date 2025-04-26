using MeetUp.Data.Base.AbstractClasses;
using static MeetUp.Enums.Enums;

namespace MeetUp.Data.Gym
{
    public class GymExerciseEO : ChildEO
    {
        public ICollection<GymSetEO> GymSetEOs { get; set; }
        public override EntityObjectType ParentRefEOType()
        {
            return EntityObjectType.GymSessionEO;
        }
        public GymSessionEO GymSessionEO { get; set; }
    }
}
