using Workout.Domain.Extensions;

namespace Workout.Domain.Data.Base.Interfaces
{
    public interface IChildDataStructure
    {
        public string ParentRef { get; set; }
        public string ParentRefType { get; set; }
        public void SetMandatoryProperties(IBaseDataStructure parentObject)
        {
            ParentRef = parentObject.ClassRef;
            ParentRefType = parentObject.EOType().GetDisplayName();
        }
    }
}
