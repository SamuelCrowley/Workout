using MeetUp.Data.Gym;
using MeetUp.Data.User;
using MeetUp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MeetUp.Exceptions;
using static MeetUp.Enums.Enums;

namespace MeetUp.Controllers.Gym
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymApiController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContext Context
        {
            get { return _context; }
        }

        public GymApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("workoutHistory")]
        public async Task<IActionResult> GetWorkoutHistory()
        {
            try
            {
                GymUserEO gymUserEO = await GetCurrentGymUser();

                List<GymSessionEO> gymSessionEOs = await _context.GymSessions
                    .Where(x => x.GymUserEO.ClassRef == gymUserEO.ClassRef && x.EndTime != DateTime.MaxValue)
                    .OrderByDescending(x => x.StartTime)
                    .Include(x => x.GymExerciseEOs)
                    .ThenInclude(x => x.GymSetEOs)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    sessions = gymSessionEOs.Select(s => new
                    {
                        id = s.ClassRef,
                        name = s.GymSessionType,
                        startTime = s.StartTime,
                        endTime = s.EndTime,
                        duration = (s.EndTime - s.StartTime).TotalMinutes,
                        exerciseCount = s.GymExerciseEOs.Count,
                        totalSets = s.GymExerciseEOs.Sum(e => e.GymSetEOs.Count)
                    })
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpGet("workoutDetails/{sessionId}")]
        public async Task<IActionResult> GetWorkoutDetails(string sessionId)
        {
            try
            {
                GymSessionEO gymSessionEO = await GetCurrentGymSession(sessionId);
                gymSessionEO = _context.GymSessions.Where(x => x.ClassRef == gymSessionEO.ClassRef)
                    .Include(x => x.GymExerciseEOs)
                    .ThenInclude(x => x.GymSetEOs)
                    .ThenInclude(x => x.GymRepetitionEOs)
                    .First();

                return Ok(new
                {
                    success = true,
                    session = new
                    {
                        id = gymSessionEO.ClassRef,
                        name = gymSessionEO.GymSessionType,
                        startTime = gymSessionEO.StartTime,
                        endTime = gymSessionEO.EndTime,
                        exercises = gymSessionEO.GymExerciseEOs.Select(x => new
                        {
                            id = x.ClassRef,
                            name = x.ExerciseName,
                            sets = x.GymSetEOs.Select(x => new
                            {
                                id = x.ClassRef,
                                reps = x.GymRepetitionEOs.Select(x => new
                                {
                                    id = x.ClassRef,
                                    weight = x.Weight,
                                    difficulty = x.Difficulty.ToString()
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpPost("startWorkout")]
        public async Task<IActionResult> StartWorkout([FromBody] string workoutName)
        {
            try
            {
                GymUserEO gymUserEO = await GetCurrentGymUser();
                bool hasActiveSession = await _context.GymSessions.AnyAsync(x => x.ClassRef == gymUserEO.ClassRef && x.EndTime == DateTime.MaxValue);

                if (hasActiveSession)
                {
                    return BadRequest("You already have an active workout session.");
                }

                GymSessionEO newGymSessionEO = new GymSessionEO
                {
                    GymUserEO = gymUserEO,
                    GymSessionType = workoutName,
                    StartTime = DateTime.UtcNow,
                };

                newGymSessionEO.SetMandatoryProperties(gymUserEO.ClassRef);

                await _context.GymSessions.AddAsync(newGymSessionEO);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Workout session started",
                    sessionId = newGymSessionEO.ClassRef
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpPost("endWorkout/{sessionId}")]
        public async Task<IActionResult> EndWorkout(string sessionId)
        {
            try
            {
                GymSessionEO gymSessionEO = await GetCurrentGymSession(sessionId);

                gymSessionEO.EndTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Workout session ended successfully"
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpPost("addExercise/{sessionId}")]
        public async Task<IActionResult> AddExercise(string sessionId, [FromBody] string exerciseName)
        {
            try
            {
                GymSessionEO gymSessionEO = await GetCurrentGymSession(sessionId);

                GymExerciseEO newGymExerciseEO = new GymExerciseEO
                {
                    GymSessionEO = gymSessionEO,
                    ExerciseName = exerciseName,
                    StartTime = DateTime.UtcNow
                };

                newGymExerciseEO.SetMandatoryProperties(gymSessionEO.ClassRef);

                await _context.GymExercises.AddAsync(newGymExerciseEO);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Exercise added successfully",
                    exerciseId = newGymExerciseEO.ClassRef
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpPost("addSet/{exerciseId}")]
        public async Task<IActionResult> AddSet(string exerciseId, [FromBody] List<RepetitionInfoDTO> repetitionInfoDTOs)
        {
            try
            {
                GymExerciseEO? gymExerciseEO = await GetCurrentGymExercise(exerciseId);

                GymSetEO gymSetEO = new GymSetEO
                {
                    GymExerciseEO = gymExerciseEO,
                    StartTime = DateTime.UtcNow
                };

                gymSetEO.SetMandatoryProperties(gymExerciseEO.ClassRef);

                await _context.GymSets.AddAsync(gymSetEO);

                foreach (RepetitionInfoDTO repInfoDTO in repetitionInfoDTOs)
                {
                    GymRepetitionEO gymRepetitionEO = new GymRepetitionEO
                    {
                        GymSetEO = gymSetEO,
                        Weight = repInfoDTO.Weight,
                        Difficulty = Enum.Parse<RepetitionDifficulty>(repInfoDTO.Difficulty),
                        Order = repInfoDTO.Order 
                    };

                    gymRepetitionEO.SetMandatoryProperties(gymSetEO.ClassRef);
                    await _context.GymRepetitions.AddAsync(gymRepetitionEO);
                }

                await _context.SaveChangesAsync();

                GymSetEO? savedGymSetEO = await _context.GymSets
                    .Include(s => s.GymRepetitionEOs)
                    .FirstOrDefaultAsync(s => s.ClassRef == gymSetEO.ClassRef);

                if (savedGymSetEO == null)
                {
                    return GymSetNotFound();
                }

                return Ok(new
                {
                    success = true,
                    message = "Set added successfully",
                    setId = gymSetEO.ClassRef,
                    reps = savedGymSetEO.GymRepetitionEOs.Select(r => new
                    {
                        id = r.ClassRef,
                        weight = r.Weight,
                        difficulty = r.Difficulty.ToString(),
                        order = r.Order
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpDelete("deleteSession/{sessionId}")]
        public async Task<IActionResult> DeleteSession(string sessionId)
        {
            try
            {
                GymSessionEO gymSessionEO = await GetCurrentGymSession(sessionId);

                _context.GymSessions.Remove(gymSessionEO); // SEC 27-04-2025 - No async version, Remove just marks for deletion in EF 
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Session deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpDelete("deleteExercise/{exerciseId}")]
        public async Task<IActionResult> DeleteExercise(string exerciseId)
        {
            try
            {
                GymExerciseEO gymExerciseEO = await GetCurrentGymExercise(exerciseId);

                _context.GymExercises.Remove(gymExerciseEO); // SEC 27-04-2025 - No async version, Remove just marks for deletion in EF 
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Exercise deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }


        [HttpDelete("deleteSet/{setId}")]
        public async Task<IActionResult> DeleteSet(string setId)
        {
            try
            {
                GymSetEO gymSetEO = await GetCurrentGymSet(setId);

                _context.GymSets.Remove(gymSetEO); // SEC 27-04-2025 - No async version, Remove just marks for deletion in EF 
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Set deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpGet("exists")]
        public async Task<IActionResult> CheckGymUserExists()
        {
            try
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Ok(new { exists = false, message = "User not authenticated" });
                }

                bool exists = await _context.GymUsers.AnyAsync(x => string.Equals(x.ParentRef, userId));

                return Ok(new
                {
                    exists,
                    message = exists
                        ? "Gym profile exists"
                        : "No gym profile found"
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGymUser()
        {
            try
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ApplicationUserEO? appUserEO = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (appUserEO == null)
                {
                    return BadRequest("User not found");
                }

                bool alreadyExists = await _context.GymUsers.AnyAsync(x => x.ParentRef == userId);
                if (alreadyExists)
                {
                    return BadRequest("Gym user already exists for this account");
                }

                GymUserEO gymUserEO = new GymUserEO
                {
                    ApplicationUserEO = appUserEO
                };

                gymUserEO.SetMandatoryProperties(appUserEO.Id);

                await _context.GymUsers.AddAsync(gymUserEO);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Gym user created successfully",
                    gymUserId = gymUserEO.ClassRef
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpGet("activeWorkout")]
        public async Task<IActionResult> GetActiveWorkout()
        {
            try
            {
                GymUserEO gymUserEO = await GetCurrentGymUser();

                GymSessionEO? activeGymSessionEO = gymUserEO.GymSessionEOs.FirstOrDefault(s => s.EndTime != DateTime.MaxValue);

                if (activeGymSessionEO == null)
                {
                    return Ok(new { success = true, hasActiveWorkout = false });
                }

                List<GymExerciseEO> gymExerciseEOs = await _context.GymExercises
                    .Where(x => x.GymSessionEO.ClassRef == activeGymSessionEO.ClassRef)
                    .Include(x => x.GymSetEOs)
                    .ThenInclude(x => x.GymRepetitionEOs)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    hasActiveWorkout = true,
                    workout = new
                    {
                        id = activeGymSessionEO.ClassRef,
                        name = activeGymSessionEO.GymSessionType,
                        startTime = activeGymSessionEO.StartTime,
                        exercises = gymExerciseEOs.Select(e => new
                        {
                            id = e.ClassRef,
                            name = e.ExerciseName,
                            startTime = e.StartTime,
                            sets = e.GymSetEOs.Select(s => new
                            {
                                id = s.ClassRef,
                                startTime = s.StartTime,
                                reps = s.GymRepetitionEOs.Select(r => new
                                {
                                    id = r.ClassRef,
                                    weight = r.Weight,
                                    difficulty = r.Difficulty.ToString()
                                }).ToList()
                            }).OrderBy(s => s.startTime).ToList()
                        }).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<GymUserEO> GetCurrentGymUser()
        {
            string? applicationEOUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            GymUserEO? currentGymUserEO = await Context.GymUsers.FirstAsync(u => u.ParentRef == applicationEOUserId);

            if (currentGymUserEO == null)
            {
                throw new BadRequestException(GymUserNotFound);
            }

            return currentGymUserEO;
        }

        public async Task<GymSessionEO> GetCurrentGymSession(string sessionId)
        {
            GymSessionEO? gymSessionEO = await Context.GymSessions.FirstAsync(x => x.ClassRef == sessionId);

            if (gymSessionEO == null)
            {
                throw new BadRequestException(GymSessionNotFound);
            }

            return gymSessionEO;
        }

        public async Task<GymExerciseEO> GetCurrentGymExercise(string exerciseId)
        {
            GymExerciseEO? gymExerciseEO = await Context.GymExercises.FirstAsync(x => x.ClassRef == exerciseId);

            if (gymExerciseEO == null)
            {
                throw new BadRequestException(GymExerciseNotFound);
            }

            return gymExerciseEO;
        }

        public async Task<GymSetEO> GetCurrentGymSet(string setId)
        {
            GymSetEO? gymSetEO = await Context.GymSets.FirstAsync(x => x.ClassRef == setId);

            if (gymSetEO == null)
            {
                throw new BadRequestException(GymSetNotFound);
            }

            return gymSetEO;
        }

        private IActionResult CatchException(Exception ex)
        {
            if (ex is APIException)
            {
                APIException apiException = ex as APIException;
                return apiException.GetError();
            }
            else
            {
                return InternalServerError(ex.Message, ex);
            }
        }

        public IActionResult GymUserNotFound()
        {
            return BadRequest($"Gym user could not be found for {Context.Users.First()?.Email ?? string.Empty}");
        }

        public IActionResult GymSessionNotFound()
        {
            return BadRequest("Workout session could not be found");
        }

        public IActionResult GymExerciseNotFound()
        {
            return BadRequest("Exercise could not be found");
        }

        public IActionResult GymSetNotFound()
        {
            return BadRequest("Set could not be found");
        }
    }

    public class RepetitionInfoDTO
    {
        public float Weight { get; set; }
        public string Difficulty { get; set; }
        public int Order { get; set; }
    }
}