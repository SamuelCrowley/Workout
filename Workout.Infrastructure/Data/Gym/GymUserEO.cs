using System.ComponentModel.DataAnnotations.Schema;
using Workout.Domain.Data.Base.AbstractClasses;
using Workout.Domain.Data.Base.Interfaces;
using Workout.Domain.Data.Interfaces.Gym;
using Workout.Domain.Enums;
using Workout.Infrastructure.Data.User;

namespace Workout.Infrastructure.Data.Gym
{
    public class GymUserEO : ChildEO, IGymUserData
    {
        public ICollection<GymSessionEO> GymSessionEOs { get; set; } = new List<GymSessionEO>();
        [ForeignKey(nameof(ParentRef))]
        public ApplicationUserEO ApplicationUserEO { get; set; }

        ICollection<IGymSessionData> IGymUserData.GymSessionEOs => GymSessionEOs.Cast<IGymSessionData>().ToList();
        IApplicationUserEO IGymUserData.ApplicationUserEO => ApplicationUserEO;

        public override EntityObjectType EOType()
        {
            return EntityObjectType.GymSessionEO;
        }

        public GymUserEO(IBaseDataStructure baseEO) : base(baseEO)
        {

        }

        public GymUserEO() : base()
        {

        }
    }
}
