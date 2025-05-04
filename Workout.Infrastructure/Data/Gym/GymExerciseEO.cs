using System.ComponentModel.DataAnnotations.Schema;
using Workout.Domain.Data.Base.AbstractClasses;
using Workout.Domain.Data.Base.Interfaces;
using Workout.Domain.Data.Interfaces.Gym;
using Workout.Domain.Enums;

namespace Workout.Infrastructure.Data.Gym
{
    public class GymExerciseEO : ChildEO, IGymExerciseData
    {
        public ICollection<GymSetEO> GymSetEOs { get; set; } = new List<GymSetEO>();

        [ForeignKey(nameof(ParentRef))]
        public GymSessionEO GymSessionEO { get; set; }
        ICollection<IGymSetData> IGymExerciseData.GymSetEOs => GymSetEOs.Cast<IGymSetData>().ToList();
        IGymSessionData IGymExerciseData.GymSessionEO => GymSessionEO;

        public override EntityObjectType EOType()
        {
            return EntityObjectType.GymExerciseEO;
        }

        public string ExerciseName { get; set; }
        public DateTime StartTime { get; set; }

        public GymExerciseEO(IBaseDataStructure baseEO) : base(baseEO)
        {
            StartTime = DateTime.Now;
        }

        public GymExerciseEO() : base()
        {

        }
    }
}
