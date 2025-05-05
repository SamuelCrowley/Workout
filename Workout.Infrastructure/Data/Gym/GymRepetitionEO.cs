using System.ComponentModel.DataAnnotations.Schema;
using Workout.Domain.Data.Base.AbstractClasses;
using Workout.Domain.Data.Base.Interfaces;
using Workout.Domain.Data.Interfaces.Gym;
using Workout.Domain.Enums;

namespace Workout.Infrastructure.Data.Gym
{
    public class GymRepetitionEO : ChildEO, IGymRepetitionData
    {
        #region EF navigation
        [ForeignKey(nameof(ParentRef))]
        public GymSetEO GymSetEO { get; set; }
        IGymSetData IGymRepetitionData.GymSetEO => GymSetEO;
        #endregion

        public override EntityObjectType EOType()
        {
            return EntityObjectType.GymRepetitionEO;
        }

        public float Weight { get; set; }
        public RepetitionDifficulty Difficulty { get; set; }

        public int Order { get; set; }

        public GymRepetitionEO(IBaseDataStructure baseEO) : base(baseEO)
        {

        }

        public GymRepetitionEO() : base()
        {

        }
    }
}
