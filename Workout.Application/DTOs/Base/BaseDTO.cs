using Workout.Domain.Data.Base.AbstractClasses;
using Workout.Domain.Data.Base.Interfaces;
using Workout.Domain.Enums;

namespace Workout.Application.DTOs.Base
{
    public abstract class BaseDTO : IBaseDataStructure
    {
        public string ClassRef { get; set; }

        public abstract EntityObjectType EOType();

        protected IBaseDataStructure _baseData;

        protected BaseDTO(IBaseDataStructure baseData)
        {
            _baseData = baseData ?? throw new ArgumentNullException(nameof(_baseData));
            SetMandatoryProperties();
        }

        public void SetMandatoryProperties()
        {
            ClassRef = _baseData.ClassRef;
        }
    }
}
