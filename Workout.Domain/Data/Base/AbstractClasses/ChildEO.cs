using Workout.Domain.Data.Base.Interfaces;
using static Workout.Domain.Consts.Consts;

namespace Workout.Domain.Data.Base.AbstractClasses
{
    public abstract class ChildEO : BaseEO, IChildDataStructure, IChildDataValidation
    {
        public string ParentRef { get; set; }

        public string ParentRefType { get; set; }

        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrWhiteSpace(ParentRef))
            {
                throw new InvalidOperationException(ERROR_PARENT_REF_REQUIRED);
            }
        }

        public ChildEO(IBaseDataStructure baseEO) : base()
        {
            IChildDataStructure childDataStructure = this;
            childDataStructure.SetMandatoryProperties(baseEO);
        }

        public ChildEO() : base()
        {

        }
    }
}
