using Microsoft.AspNetCore.Mvc.RazorPages;
using static MeetUp.Enums.Enums;

namespace MeetUp.Views
{
    public class ChatModel : PageModel
    {
        public Dictionary<RepetitionDifficulty, string> DifficultyOptions { get; } =
        Enum.GetValues(typeof(RepetitionDifficulty))
           .Cast<RepetitionDifficulty>()
           .Where(d => d != RepetitionDifficulty.Unknown) 
           .ToDictionary(
               k => k,
               v => v.ToString()
           );

        public Dictionary<RepetitionDifficulty, string> DifficultyClasses { get; } = new()
        {
            [RepetitionDifficulty.Warmup] = "difficulty-1",
            [RepetitionDifficulty.Easy] = "difficulty-2",
            [RepetitionDifficulty.Moderate] = "difficulty-3",
            [RepetitionDifficulty.Difficult] = "difficulty-4",
            [RepetitionDifficulty.Extreme] = "difficulty-5",
            [RepetitionDifficulty.Failed] = "difficulty-6"
        };

        public void OnGet()
        {
        }
    }
}
