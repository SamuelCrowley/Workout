using MeetUp.Data.Gym;
using MeetUp.Data.User;
using MeetUp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MeetUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymUserApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUserEO> _userManager;

        public GymUserApiController(ApplicationDbContext context, UserManager<ApplicationUserEO> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("workouts")]
        public async Task<IActionResult> GetWorkouts()
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                GymUserEO? gymUser = await _context.GymUsers.FirstOrDefaultAsync(x => string.Equals(x.ParentRef, userId));

                if (gymUser == null)
                {
                    return BadRequest(new { success = false, message = "Cannot get workouts, as no profile exists" });
                }

                return Ok(new
                {
                    success = true,
                    workoutCount = gymUser.GymSessionEOs.Count,
                    message = gymUser.GymSessionEOs.Count > 0
                        ? $"You've completed {gymUser.GymSessionEOs.Count} workouts"
                        : "No workouts logged yet"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching workouts",
                    error = ex.Message
                });
            }
        }

        [HttpPost("startWorkout")]
        public async Task<IActionResult> StartWorkout()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            GymUserEO gymUserEO = await _context.GymUsers.FirstOrDefaultAsync(u => u.ParentRef == userId);
            if (gymUserEO == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "User not found."
                });
            }

            GymSessionEO gymSessionEO = new GymSessionEO
            {
                GymUserEO = gymUserEO,
            };

            gymSessionEO.SetMandatoryProperties(gymUserEO.ClassRef);

            _context.GymSessions.Add(gymSessionEO);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Gym session started"
            });
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
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error checking gym user status",
                    error = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> CreateGymUser()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUserEO appUserEO = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (appUserEO == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "User not found."
                });
            }

            bool alreadyExists = await _context.GymUsers.AnyAsync(x => x.ParentRef == userId);
            if (alreadyExists)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Gym user already exists for this account."
                });
            }

            GymUserEO gymUser = new GymUserEO
            {
                ApplicationUserEO = appUserEO
            };

            gymUser.SetMandatoryProperties(appUserEO.Id);

            _context.GymUsers.Add(gymUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Gym user created successfully",
                gymUserId = gymUser.ClassRef
            });
        }
    }
}