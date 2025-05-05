using Microsoft.AspNetCore.Mvc;
using Workout.Application.DTOs.Gym;
using Workout.Application.Services.Gym;

namespace Workout.Web.Controllers.Gym
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymApiController : BaseApiController
    {
        private readonly IWorkoutService _workoutService;

        public GymApiController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [HttpGet("workoutHistory")]
        public async Task<IActionResult> GetWorkoutHistory()
        {
            try
            {
                var gymUser = await _workoutService.GetCurrentGymUser();

                if (gymUser == null)
                {
                    throw new Exception("Gym user failed to find");
                }

                var sessions = await _workoutService.GetSessionHistoryByUser(gymUser.ClassRef);

                return Ok(new { success = true, sessions });
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
                GymSessionDTO? gymSessionDTO = await _workoutService.GetSessionDetails(sessionId);

                if (gymSessionDTO == null)
                {
                    throw new Exception("Gym session failed to find");
                }

                return Ok(new
                {
                    success = true,
                    session = new
                    {
                        id = gymSessionDTO.ClassRef,
                        name = gymSessionDTO.GymSessionType,
                        startTime = gymSessionDTO.StartTime,
                        endTime = gymSessionDTO.EndTime,
                        exercises = gymSessionDTO.GymExerciseDTOs.Select(x => new
                        {
                            id = x.ClassRef,
                            name = x.ExerciseName,
                            sets = x.GymSetDTOs.OrderBy(x => x.StartTime).Select(x => new
                            {
                                id = x.ClassRef,
                                reps = x.GymRepetitionDTOs.Select(x => new
                                {
                                    id = x.ClassRef,
                                    weight = x.Weight,
                                    difficulty = x.Difficulty.ToString(),
                                    order = x.Order,
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
                bool hasActiveSession = await _workoutService.DoesUserHaveActiveWorkout();

                if (hasActiveSession)
                {
                    throw new Exception("Gym user already has active session");
                }

                GymSessionDTO? newGymSessionDTO = await _workoutService.CreateNewGymSession(workoutName);

                if (newGymSessionDTO == null)
                {
                    throw new Exception("Gym session failed to start");
                }

                return Ok(new
                {
                    success = true,
                    message = "Workout session started",
                    sessionId = newGymSessionDTO.ClassRef
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
                GymSessionDTO? gymSessionDTO = await _workoutService.EndGymSession(sessionId);

                if (gymSessionDTO == null)
                {
                    throw new Exception("Gym session failed to end");
                }

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
                GymExerciseDTO? gymExerciseDTO = await _workoutService.CreateExercise(sessionId, exerciseName);

                if (gymExerciseDTO == null)
                {
                    throw new Exception("Gym exercise failed to create");
                }

                return Ok(new
                {
                    success = true,
                    message = "Exercise added successfully",
                    exerciseId = gymExerciseDTO.ClassRef
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpPost("addSet/{exerciseId}")]
        public async Task<IActionResult> AddSet(string exerciseId, [FromBody] List<GymRepInfoDTO> repetitionInfoDTOs)
        {
            try
            {
                GymSetDTO? gymSetDTO = await _workoutService.CreateSet(exerciseId, repetitionInfoDTOs);

                if (gymSetDTO == null)
                {
                    throw new Exception("Gym set failed to create");
                }

                return Ok(new
                {
                    success = true,
                    message = "Set added successfully",
                    setId = gymSetDTO.ClassRef,
                    reps = gymSetDTO.GymRepetitionDTOs.Select(x => new
                    {
                        id = x.ClassRef,
                        weight = x.Weight,
                        difficulty = x.Difficulty.ToString(),
                        order = x.Order
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        [HttpPatch("editSet/{setId}")]
        public async Task<IActionResult> EditSet(string setId, [FromBody] List<GymRepInfoDTO> repetitionInfoDTOs)
        {
            try
            {
                GymSetDTO? gymSetDTO = await _workoutService.EditSet(setId, repetitionInfoDTOs);

                if (gymSetDTO == null)
                {
                    throw new Exception("Gym set failed to edit");
                }

                return Ok(new
                {
                    success = true,
                    message = "Set edited successfully",
                    setId = gymSetDTO.ClassRef,
                    reps = gymSetDTO.GymRepetitionDTOs.Select(r => new
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
                GymSessionDTO? gymSessionDTO = await _workoutService.DeleteSession(sessionId);

                if (gymSessionDTO == null)
                {
                    throw new Exception("Gym session failed to delete");
                }
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
        public async Task<IActionResult> DeleteExercise(string ExerciseId)
        {
            try
            {
                GymExerciseDTO? gymExerciseDTO = await _workoutService.DeleteExercise(ExerciseId);

                if (gymExerciseDTO == null)
                {
                    throw new Exception("Gym exercise failed to delete");
                }

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
                GymSetDTO? gymSetDTO = await _workoutService.DeleteSet(setId);

                if (gymSetDTO == null)
                {
                    throw new Exception("Gym set failed to delete");
                }

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
                bool exists = await _workoutService.DoesGymUserExistForUser();

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

        [HttpPost("createGymUser")]
        public async Task<IActionResult> CreateGymUser()
        {
            try
            {
                GymUserDTO? gymUserDTO = await _workoutService.CreateGymUser();

                if (gymUserDTO == null)
                {
                    throw new Exception("Gym User unexpectedly not found");
                }

                return Ok(new
                {
                    success = true,
                    message = "Gym user created successfully",
                    gymUserId = gymUserDTO.ClassRef
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
                GymSessionDTO? mostRecentGymSessionDTO = await _workoutService.GetMostRecentSession();

                // SEC 04-May-2025 - This situation can be hanlded better once a better error manager is implemented
                //if (mostRecentGymSessionDTO == null)
                //{
                //    throw new BadRequestException(FailedToFindGymSession);
                //}

                if (mostRecentGymSessionDTO == null || mostRecentGymSessionDTO.EndTime != null)
                {
                    return Ok(new { success = true, hasActiveWorkout = false });
                }

                return Ok(new
                {
                    success = true,
                    hasActiveWorkout = true,
                    workout = new
                    {
                        id = mostRecentGymSessionDTO.ClassRef,
                        name = mostRecentGymSessionDTO.GymSessionType,
                        startTime = mostRecentGymSessionDTO.StartTime,
                        exercises = mostRecentGymSessionDTO.GymExerciseDTOs.Select(x => new
                        {
                            id = x.ClassRef,
                            name = x.ExerciseName,
                            startTime = x.StartTime,
                            sets = x.GymSetDTOs.Select(x => new
                            {
                                id = x.ClassRef,
                                startTime = x.StartTime,
                                reps = x.GymRepetitionDTOs.Select(x => new
                                {
                                    id = x.ClassRef,
                                    weight = x.Weight,
                                    difficulty = x.Difficulty.ToString()
                                }).ToList()
                            }).OrderBy(x => x.startTime).ToList()
                        }).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<GymSessionDTO?> GetCurrentGymSession(string sessionId)
        {
            GymSessionDTO? gymSessionDTO = null;

            try
            {
                gymSessionDTO = await _workoutService.GetGymSessionByClassRef(sessionId);

            }
            catch (Exception ex)
            {
                CatchException(ex);
            }

            return gymSessionDTO;
        }

        public async Task<GymExerciseDTO?> GetCurrentGymExercise(string exerciseId)
        {
            GymExerciseDTO? gymExerciseDTO = null;
            try
            {
                gymExerciseDTO = await _workoutService.GetGymExerciseByClassRef(exerciseId);
                return gymExerciseDTO;
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }

            return gymExerciseDTO;
        }

        public async Task<GymSetDTO?> GetCurrentGymSet(string setId)
        {
            GymSetDTO? gymSetEO = null;

            try
            {
                gymSetEO = await _workoutService.GetGymSetByClassRef(setId);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }

            return gymSetEO;
        }
    }
}