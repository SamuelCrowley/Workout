using Workout.Domain.Enums;

namespace Workout.Domain.Data.Base.Interfaces
{
    public interface IBaseDataStructure
    {
        public string ClassRef { get; set; }
        public EntityObjectType EOType();

        public void SetMandatoryProperties()
        {
            ClassRef = Guid.NewGuid().ToString();
        }
    }
}
