namespace MeetUp.Data.Base.Interfaces
{
    public interface IChildEO
    {
        public string ParentRef { get; }
        public string ParentRefType { get; }
        public void SetMandatoryProperties();
        public void Validate();
    }
}
