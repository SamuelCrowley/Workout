using MeetUp.Data.Base.AbstractClasses;
using static MeetUp.Enums.Enums;

namespace MeetUp.Data.Gym
{
    public class GymSetEO : ChildEO
    {
        public ICollection<GymRepetitionEO> GymRepetitionEOs { get; set; }
        public override EntityObjectType ParentRefEOType()
        {
            return EntityObjectType.GymSetEO;
        }
        public GymExerciseEO GymExerciseEO { get; set; }
    }
}
