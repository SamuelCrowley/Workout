using MeetUp.Data.Base.AbstractClasses;
using static MeetUp.Enums.Enums;

namespace MeetUp.Data.Gym
{
    public class GymSetEO : ChildEO
    {
        public ICollection<GymRepetitionEO> GymRepetitionEOs { get; set; }
        public override EntityObjectType ParentRefEOType()
        {
            return EntityObjectType.GymExerciseEO;
        }
        public GymExerciseEO GymExerciseEO { get; set; }

        public DateTime StartTime { get; set; }
    }
}
