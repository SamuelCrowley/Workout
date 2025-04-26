using MeetUp.Data.Base.AbstractClasses;
using static MeetUp.Enums.Enums;

namespace MeetUp.Data.Gym
{
    public class GymRepetitionEO : ChildEO
    {
        public override EntityObjectType ParentRefEOType()
        {
            return EntityObjectType.GymSetEO;
        }
        public GymSetEO GymSetEO { get; set; }

        public int Weight { get; set; }
        public RepetitionDifficulty Difficulty { get; set; }
    }
}
