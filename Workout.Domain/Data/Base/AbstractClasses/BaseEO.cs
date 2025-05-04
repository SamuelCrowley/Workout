using Workout.Domain.Data.Base.Interfaces;
using Workout.Domain.Enums;
using static Workout.Domain.Consts.Consts;

namespace Workout.Domain.Data.Base.AbstractClasses
{
    public abstract class BaseEO : IBaseDataStructure, IBaseDataValidation
    {
        public string ClassRef { get; set; }

        public abstract EntityObjectType EOType();

        

        public BaseEO()
        {
            IBaseDataStructure baseDataStructure = this;
            baseDataStructure.SetMandatoryProperties();
        }

        public virtual void Validate()
        {
            if (string.IsNullOrWhiteSpace(ClassRef))
            {
                throw new InvalidOperationException(ERROR_CLASS_REF_REQUIRED);
            }
        }
    }
}
