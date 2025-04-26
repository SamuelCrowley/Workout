namespace MeetUp.Data.Base.Interfaces
{
    public interface IBaseEO
    {
        public string ClassRef { get; }
        public void SetMandatoryProperties();
        public void Validate();
    }
}
