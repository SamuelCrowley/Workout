using System.ComponentModel.DataAnnotations;

namespace MeetUp.Enums
{
    public partial class Enums
    {
        public enum EntityObjectType
        {
            [Display(Name = "Unknown")]
            Unknown,
            [Display(Name = "AspNetUsers")]
            ApplicationUserEO = 10001,
            [Display(Name = "Gym_Users")]
            GymUserEO = 20001,
            [Display(Name = "Gym_Sessions")]
            GymSessionEO = 20002,
            [Display(Name = "Gym_Exercises")]
            GymExerciseEO = 20003,
            [Display(Name = "Gym_Sets")]
            GymSetEO = 20004,
            [Display(Name = "Gym_Repetitions")]
            GymRepetitionEO = 20005,
        }
    }
}
