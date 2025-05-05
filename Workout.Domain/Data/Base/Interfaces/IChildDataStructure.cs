using Workout.Domain.Extensions;

namespace Workout.Domain.Data.Base.Interfaces
{
    /// <summary>
    /// SEC 05-May-2025
    /// This interface describes data/methods that should be implemented by all of the EOs in the system
    /// that sit below other EOs in the table structure (i.e those that have a parent), with the exception those that inherit from
    /// external classes such as IdentityUser
    /// </summary>
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
