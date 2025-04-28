using MeetUp.Data.Base.AbstractClasses;
using static MeetUp.Enums.Enums;

namespace MeetUp.Data.Gym
{
    public class GymSessionEO : ChildEO
    {
        public GymUserEO GymUserEO { get; set; }
        public override EntityObjectType ParentRefEOType()
        {
            return EntityObjectType.GymUserEO;
        }
        public ICollection<GymExerciseEO> GymExerciseEOs { get; set; }

        public string GymSessionType { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public GymSessionEO() : base()
        {
            EndTime = DateTime.MaxValue;
        }
    }
}
