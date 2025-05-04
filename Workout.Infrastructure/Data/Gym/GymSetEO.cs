using System.ComponentModel.DataAnnotations.Schema;
using Workout.Domain.Data.Base.AbstractClasses;
using Workout.Domain.Data.Base.Interfaces;
using Workout.Domain.Data.Interfaces.Gym;
using Workout.Domain.Enums;

namespace Workout.Infrastructure.Data.Gym
{
    public class GymSetEO : ChildEO, IGymSetData
    {
        #region EF navigation
        public ICollection<GymRepetitionEO> GymRepetitionEOs { get; set; } = new List<GymRepetitionEO>();
        [ForeignKey(nameof(ParentRef))]
        public GymExerciseEO GymExerciseEO { get; set; }
        ICollection<IGymRepetitionData> IGymSetData.GymRepetitionEOs =>
            GymRepetitionEOs.Cast<IGymRepetitionData>().ToList();
        IGymExerciseData IGymSetData.GymExerciseEO => GymExerciseEO;
        #endregion
        public override EntityObjectType EOType()
        {
            return EntityObjectType.GymSetEO;
        }

        public DateTime StartTime { get; set; }

        public GymSetEO(IBaseDataStructure baseEO) : base(baseEO)
        {
            StartTime = DateTime.UtcNow;
        }

        public GymSetEO() : base()
        {

        }
    }
}
