using Workout.Domain.Enums;

namespace Workout.Domain.Data.Base.Interfaces
{
    /// <summary>
    /// SEC 05-May-2025
    /// This interface describes data/methods that should be implemented by all of the EOs in the system, with the exception those that inherit from
    /// external classes such as IdentityUser
    /// </summary>
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
