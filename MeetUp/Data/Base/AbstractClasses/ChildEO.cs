using MeetUp.Data.Base.Interfaces;
using static MeetUp.Enums.Enums;
using static MeetUp.Consts.Consts;
using MeetUp.Extensions;

namespace MeetUp.Data.Base.AbstractClasses
{
    public abstract class ChildEO : BaseEO, IChildEO
    {
        public string ParentRef { get; private set; }

        public string ParentRefType { get; private set; }
        public abstract EntityObjectType ParentRefEOType();

        public void SetMandatoryProperties(string parentRef)
        {
            ParentRef = parentRef;
            ParentRefType = ParentRefEOType().GetDisplayName();
        }

        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrWhiteSpace(ParentRef))
            {
                throw new InvalidOperationException(ERROR_PARENT_REF_REQUIRED);
            }
        }
    }
}
