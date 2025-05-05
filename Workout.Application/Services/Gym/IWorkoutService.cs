using Workout.Application.DTOs.Gym;
namespace Workout.Application.Services.Gym
{
    /// <summary>
    /// SEC 05-May-2025 
    /// Definition of the service methods, this is the 'what' of the service rather than the 'how', which is handled by the Infrastructure subsystem
    /// </summary>
    public interface IWorkoutService
    {
        Task<GymUserDTO?> GetGymUserByParentRef(string parentRef);
        Task<GymSessionDTO?> GetGymSessionByClassRef(string classRef);
        Task<GymExerciseDTO?> GetGymExerciseByClassRef(string classRef);
        Task<GymSetDTO?> GetGymSetByClassRef(string classRef);
        Task<GymUserDTO?> GetCurrentGymUser();
        Task<IReadOnlyList<GymSessionDTO>> GetSessionHistoryByUser(string gymUserClassRef);
        Task<GymSessionDTO?> GetSessionDetails(string gymSessionClassRef);
        Task<bool> DoesUserHaveActiveWorkout();
        Task<GymSessionDTO?> CreateNewGymSession(string gymSessionType);
        Task<GymSessionDTO?> EndGymSession(string gymSessionClassRef);
        Task<GymExerciseDTO?> CreateExercise(string gymSessionClassRef, string exerciseName);
        Task<GymSetDTO?> CreateSet(string exerciseClassRef, List<GymRepInfoDTO> repetitionInfoDTOs);
        Task<GymSetDTO?> EditSet(string gymSetClassReference, List<GymRepInfoDTO> repetitionInfoDTOs);
        Task<GymSessionDTO?> DeleteSession(string gymSessionClassReference);
        Task<GymSetDTO?> DeleteSet(string gymSetClassRef);
        Task<GymExerciseDTO?> DeleteExercise(string gymExerciseClassRef);
        Task<bool> DoesGymUserExistForUser();
        Task<GymUserDTO?> CreateGymUser();
        Task<GymSessionDTO?> GetMostRecentSession();
    }
}
