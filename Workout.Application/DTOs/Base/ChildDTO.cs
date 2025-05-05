using Workout.Domain.Data.Base.Interfaces;

namespace Workout.Application.DTOs.Base
{
    public abstract class ChildDTO : BaseDTO, IChildDataStructure
    {
        public string ParentRef { get; set; }
        public string ParentRefType { get; set; }

        protected ChildDTO(IBaseDataStructure baseData) : base(baseData)
        {
            IChildDataStructure childDataStructure = this;
            childDataStructure.SetMandatoryProperties(baseData);
        }
    }
}
