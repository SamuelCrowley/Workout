using MeetUp.Data.Base.Interfaces;
using static MeetUp.Consts.Consts;

namespace MeetUp.Data.Base.AbstractClasses
{
    public abstract class BaseEO : IBaseEO
    {
        public string ClassRef { get; private set; }

        public void SetMandatoryProperties()
        {
            ClassRef = Guid.NewGuid().ToString();
        }

        public BaseEO()
        {
            SetMandatoryProperties();
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
