using System.ComponentModel.DataAnnotations.Schema;
using Workout.Domain.Data.Base.AbstractClasses;
using Workout.Domain.Data.Interfaces.Gym;
using Workout.Domain.Enums;

namespace Workout.Infrastructure.Data.Gym
{
    public class GymSessionEO : ChildEO, IGymSessionData
    {
        public ICollection<GymExerciseEO> GymExerciseEOs { get; set; } = new List<GymExerciseEO>();
        [ForeignKey(nameof(ParentRef))]
        public GymUserEO GymUserEO { get; set; }

        ICollection<IGymExerciseData> IGymSessionData.GymExerciseEOs => 
            GymExerciseEOs.Cast<IGymExerciseData>().ToList();
        IGymUserData IGymSessionData.GymUserEO => GymUserEO;


        public override EntityObjectType EOType()
        {
            return EntityObjectType.GymSessionEO;
        }

        public string GymSessionType { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }


        public GymSessionEO(GymUserEO gymUserEO) : base(gymUserEO)
        {
            StartTime = DateTime.UtcNow;
            EndTime = null;
        }

        public GymSessionEO() : base()
        {

        }
    }
}
