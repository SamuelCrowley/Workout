using MeetUp.Data.Base.AbstractClasses;
using MeetUp.Data.User;
using static MeetUp.Enums.Enums;

namespace MeetUp.Data.Gym
{
    public class GymUserEO : ChildEO
    {
        public ICollection<GymSessionEO> GymSessionEOs { get; set; }
        public ApplicationUserEO ApplicationUserEO { get; set; }
        public override EntityObjectType ParentRefEOType()
        {
            return EntityObjectType.ApplicationUserEO;
        }
    }
}
