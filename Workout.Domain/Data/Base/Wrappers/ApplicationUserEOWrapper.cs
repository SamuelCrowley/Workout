using Workout.Domain.Data.Base.Interfaces;
using Workout.Domain.Enums;

namespace Workout.Domain.Data.Base.Wrappers
{
    /// <summary>
    /// SEC 03-Mar-2025 
    /// Explicitly implements IBaseDataStructure, taking IApplicationUserEO as a parameter for the constructor
    /// This allows this class to be injected into the constructor for objects that have it as a parent, such as GymUserEO, regular
    /// IApplicationUserEOs cannot be injected into that constructor due to not implementing IBaseDataStructure
    /// </summary>
    public class ApplicationUserEOWrapper : IBaseDataStructure
    {
        public string ClassRef { get; set; }

        public EntityObjectType EOType()
        {
            return EntityObjectType.ApplicationUserEO;
        }

        public ApplicationUserEOWrapper(IApplicationUserEO applicationUserEO)
        {
            ClassRef = applicationUserEO.Id;
        }
    }
}
