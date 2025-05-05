using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Workout.Application.DTOs.Gym;
using Workout.Application.Services.Gym;
using Workout.Domain.Data.Base.Wrappers;
using Workout.Domain.Enums;
using Workout.Infrastructure.Data;
using Workout.Infrastructure.Data.Gym;
using Workout.Infrastructure.Data.User;

namespace Workout.Infrastructure.Services
{
    /// <summary>
    /// SEC 05-May-2025
    /// Concrete implementation of IWorkoutService from Workout.Application, this is the 'how' of the service, not the 'what'
    /// </summary>
    public class WorkoutService : IWorkoutService
    {
        private readonly ClaimsPrincipal _user;
        private readonly ApplicationDbContext _context;
        public WorkoutService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _user = httpContextAccessor.HttpContext?.User
                ?? throw new InvalidOperationException("HTTP context user error");
        }

        public async Task<GymUserDTO?> GetCurrentGymUser()
        {
            string? applicationUserID = GetApplicationUserId();
            GymUserDTO? currentGymUserDTO = null;

            if (applicationUserID != null)
            {
                currentGymUserDTO = await GetGymUserByParentRef(applicationUserID);
            }

            return currentGymUserDTO == null ? null : currentGymUserDTO;
        }

        private string? GetApplicationUserId()
        {
            return _user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<GymUserDTO?> GetGymUserByParentRef(string parentRef)
        {
            GymUserEO? gymUserEO = await _context.GymUsers.FirstOrDefaultAsync(u => u.ParentRef == parentRef);

            return gymUserEO == null ? null : new GymUserDTO(gymUserEO);
        }

        public async Task<GymSessionDTO?> GetGymSessionByClassRef(string classRef)
        {
            GymSessionEO? gymSessionEO = await _context.GymSessions.FirstAsync(x => x.ClassRef == classRef);

            return gymSessionEO == null ? null : new GymSessionDTO(gymSessionEO);
        }

        public async Task<GymExerciseDTO?> GetGymExerciseByClassRef(string classRef)
        {
            GymExerciseEO? gymExerciseEO = await _context.GymExercises.FirstAsync(x => x.ClassRef == classRef);

            return gymExerciseEO == null ? null : new GymExerciseDTO(gymExerciseEO);
        }

        public async Task<GymSetDTO?> GetGymSetByClassRef(string classRef)
        {
            GymSetEO? gymSetEO = await _context.GymSets.FirstAsync(x => x.ClassRef == classRef);

            return gymSetEO == null ? null : new GymSetDTO(gymSetEO);
        }

        public async Task<IReadOnlyList<GymSessionDTO>> GetSessionHistoryByUser(string gymUserClassRef)
        {
            List<GymSessionEO> gymSessionEOs = await _context.GymSessions
                .Where(x => x.GymUserEO.ClassRef == gymUserClassRef && x.EndTime != null)
                .OrderByDescending(x => x.StartTime)
                .Include(x => x.GymExerciseEOs)
                .ThenInclude(e => e.GymSetEOs)
                .ToListAsync();

            return gymSessionEOs.Select(x => new GymSessionDTO(x)).ToList();
        }

        public async Task<GymSessionDTO?> GetSessionDetails(string gymSessionClassRef)
        {
            GymSessionEO? gymSessionEO = await _context.GymSessions.Where(x => x.ClassRef == gymSessionClassRef)
                .Include(x => x.GymExerciseEOs)
                .ThenInclude(x => x.GymSetEOs)
                .ThenInclude(x => x.GymRepetitionEOs)
                .FirstOrDefaultAsync();

            return gymSessionEO == null ? null : new GymSessionDTO(gymSessionEO);
        }

        public async Task<bool> DoesUserHaveActiveWorkout()
        {
            bool returnValue = false;
            string? applicationUserId = GetApplicationUserId();

            if (applicationUserId != null)
            {
                GymUserEO? gymUserEO = await _context.GymUsers.Where(u => u.ParentRef == applicationUserId).Include(x => x.GymSessionEOs).FirstOrDefaultAsync();
                if (gymUserEO != null)
                {
                    returnValue = gymUserEO.GymSessionEOs.Any(x => x.EndTime == null);
                }
            }

            return returnValue;
        }

        public async Task<GymSessionDTO?> CreateNewGymSession(string gymSessionType)
        {
            string? applicationUserID = GetApplicationUserId();

            if (applicationUserID == null)
            {
                return null;
            }

            GymUserEO? currentGymUserEO = await _context.GymUsers.Where(x => x.ParentRef == applicationUserID).FirstOrDefaultAsync();

            if (currentGymUserEO == null)
            {
                return null;
            }

            GymSessionEO newGymSessionEO = new GymSessionEO(currentGymUserEO)
            {
                GymSessionType = gymSessionType
            };

            await _context.GymSessions.AddAsync(newGymSessionEO);
            await _context.SaveChangesAsync();

            return new GymSessionDTO(newGymSessionEO);
        }

        public async Task<GymSessionDTO?> EndGymSession(string gymSessionClassRef)
        {
            GymSessionEO? gymSessionEO = await _context.GymSessions.Where(x => x.ClassRef == gymSessionClassRef).FirstOrDefaultAsync();

            if (gymSessionEO == null) 
            {  
                return null;
            }

            if (gymSessionEO.EndTime != null)
            {
                return null;
            }

            gymSessionEO.EndTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new GymSessionDTO(gymSessionEO);
        }

        public async Task<GymExerciseDTO?> CreateExercise(string gymSessionClassRef, string exerciseName)
        {
            GymSessionEO? gymSessionEO = await _context.GymSessions.Where(x => x.ClassRef == gymSessionClassRef).FirstOrDefaultAsync();

            if (gymSessionEO == null)
            {
                return null; // SEC 03-Mar-2025 - Goal is to later implement throwing BadRequest -> Catch in API and then determine if 400 or 500
            }

            GymExerciseEO newGymExerciseEO = new GymExerciseEO(gymSessionEO)
            {
                ExerciseName = exerciseName
            };

            await _context.GymExercises.AddAsync(newGymExerciseEO);
            await _context.SaveChangesAsync();

            return new GymExerciseDTO(newGymExerciseEO);
        }

        public async Task<GymSetDTO?> CreateSet(string exerciseClassRef, List<GymRepInfoDTO> repetitionInfoDTOs)
        {
            GymExerciseEO? gymExerciseEO = await _context.GymExercises.Where(x => x.ClassRef == exerciseClassRef).FirstOrDefaultAsync();

            if (gymExerciseEO == null)
            {
                return null;
            }

            GymSetEO gymSetEO = new GymSetEO(gymExerciseEO)
            {
                GymExerciseEO = gymExerciseEO
            };

            await _context.GymSets.AddAsync(gymSetEO);

            foreach (GymRepInfoDTO repInfoDTO in repetitionInfoDTOs)
            {
                GymRepetitionEO gymRepetitionEO = new GymRepetitionEO(gymSetEO)
                {
                    GymSetEO = gymSetEO,
                    Weight = repInfoDTO.Weight,
                    Difficulty = (RepetitionDifficulty)Enum.Parse(typeof(RepetitionDifficulty), repInfoDTO.Difficulty),
                    Order = repInfoDTO.Order
                };

                await _context.GymRepetitions.AddAsync(gymRepetitionEO);
            }

            await _context.SaveChangesAsync();

            return new GymSetDTO(gymSetEO);
        }

        public async Task<GymSetDTO?> EditSet(string gymSetClassRef, List<GymRepInfoDTO> repetitionInfoDTOs)
        {
            GymSetEO? gymSetEO = await _context.GymSets.Where(x => x.ClassRef == gymSetClassRef).FirstOrDefaultAsync();

            if (gymSetEO == null)
            {
                return null;
            }

            gymSetEO.GymRepetitionEOs.Clear();

            foreach (GymRepInfoDTO repInfoDTO in repetitionInfoDTOs)
            {
                GymRepetitionEO gymRepetitionEO = new GymRepetitionEO(gymSetEO)
                {
                    GymSetEO = gymSetEO,
                    Weight = repInfoDTO.Weight,
                    Difficulty = (RepetitionDifficulty)Enum.Parse(typeof(RepetitionDifficulty), repInfoDTO.Difficulty),
                    Order = repInfoDTO.Order
                };

                await _context.GymRepetitions.AddAsync(gymRepetitionEO);
            }

            await _context.SaveChangesAsync();

            return new GymSetDTO(gymSetEO);
        }

        public async Task<GymSessionDTO?> DeleteSession(string gymSessionClassRef)
        {
            GymSessionEO? gymSessionEO = await _context.GymSessions.Where(x => x.ClassRef == gymSessionClassRef).FirstOrDefaultAsync();

            if (gymSessionEO == null)
            {
                return null;
            }

            _context.GymSessions.Remove(gymSessionEO); // SEC 27-04-2025 - No async version, Remove just marks for deletion in EF 
            await _context.SaveChangesAsync();

            return new GymSessionDTO(gymSessionEO);
        }

        public async Task<GymExerciseDTO?> DeleteExercise(string gymExerciseClassRef)
        {
            GymExerciseEO? gymExerciseEO = await _context.GymExercises.Where(x => x.ClassRef == gymExerciseClassRef).FirstOrDefaultAsync();

            if (gymExerciseEO == null)
            {
                return null;
            }

            _context.GymExercises.Remove(gymExerciseEO); // SEC 27-04-2025 - No async version, Remove just marks for deletion in EF 
            await _context.SaveChangesAsync();

            return new GymExerciseDTO(gymExerciseEO);
        }

        public async Task<GymSetDTO?> DeleteSet(string gymSetClassRef)
        {
            GymSetEO? gymSetEO = await _context.GymSets.Where(x => x.ClassRef == gymSetClassRef).FirstOrDefaultAsync();

            if (gymSetEO == null)
            {
                return null;
            }

            _context.GymSets.Remove(gymSetEO); // SEC 27-04-2025 - No async version, Remove just marks for deletion in EF 
            await _context.SaveChangesAsync();

            return new GymSetDTO(gymSetEO);
        }

        public async Task<bool> DoesGymUserExistForUser()
        {
            string? userId = _user.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _context.GymUsers.AnyAsync(x => string.Equals(x.ParentRef, userId));
        }

        public async Task<GymUserDTO?> CreateGymUser()
        {
            string? userId = _user.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUserEO? applicationUserEO = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (applicationUserEO == null)
            {
                return null;
            }

            bool alreadyExists = await _context.GymUsers.AnyAsync(x => x.ParentRef == userId);
            if (alreadyExists)
            {
                return null;
            }

            ApplicationUserEOWrapper applicationUserEOWrapper = new ApplicationUserEOWrapper(applicationUserEO);

            GymUserEO gymUserEO = new GymUserEO(applicationUserEOWrapper)
            {
                ApplicationUserEO = applicationUserEO
            };

            await _context.GymUsers.AddAsync(gymUserEO);
            await _context.SaveChangesAsync();

            return new GymUserDTO(gymUserEO);
        }

        public async Task<GymSessionDTO?> GetMostRecentSession()
        {
            string? applicationUserID = GetApplicationUserId();

            if (applicationUserID == null)
            {
                return null;
            }

            GymUserEO? currentGymUserEO = await _context.GymUsers.Where(x => x.ParentRef == applicationUserID)
                                                                 .Include(x => x.GymSessionEOs)
                                                                 .ThenInclude(x => x.GymExerciseEOs)
                                                                 .ThenInclude(x => x.GymSetEOs)
                                                                 .ThenInclude(x => x.GymRepetitionEOs)
                                                                 .FirstOrDefaultAsync();

            if (currentGymUserEO == null)
            {
                return null;
            }

            GymSessionEO? mostRecentGymSessionEO = currentGymUserEO.GymSessionEOs.OrderByDescending(x => x.EndTime ?? DateTime.MaxValue).FirstOrDefault();

            if (mostRecentGymSessionEO == null)
            {
                return null;
            }

            return new GymSessionDTO(mostRecentGymSessionEO);
        }
    }
}
