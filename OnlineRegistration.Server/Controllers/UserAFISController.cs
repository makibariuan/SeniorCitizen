

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRegistration.Server.Data; // Use AppDbContext
using OnlineRegistration.Server.Models;
using OnlineRegistration.Server.DTOs;
using System.Threading.Tasks;

namespace OnlineRegistration.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAFISController : ControllerBase
    {
        // Changed to AppDbContext for citizen data
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserAFISController(AppDbContext context, IConfiguration config)
        {
            _context = context; // Renamed to _context for convention
            _config = config;
        }

        // POST api/AFIS/hit
        /// <summary>
        /// Updates a citizen's biometric enrollment record after AFIS reports a duplicate match (Hit).
        /// </summary>
        [HttpPost("hit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCitizenWithHit([FromBody] PersonWithHitDto personWithHit)
        {
            // Find the citizen record using the PersonID (PersonA in the AFIS payload)
            var citizen = await _context.Citizens
                .FirstOrDefaultAsync(c => c.Id == personWithHit.PersonA);

            if (citizen == null)
            {
                return NotFound(new { message = $"Citizen with ID {personWithHit.PersonA} not found." });
            }

            // Find the LATEST (Status=2/Locked) biometric record associated with this citizen
            var enrollment = await _context.BiometricEnrollments
                .Where(b => b.PersonId == personWithHit.PersonA)
                .OrderByDescending(b => b.DateUpload)
                .FirstOrDefaultAsync();

            if (enrollment == null)
            {
                // Handle case where citizen exists but enrollment record is missing
                return NotFound(new { message = $"Active biometric enrollment for Citizen ID {personWithHit.PersonA} not found." });
            }

            // --- Update Biometric Enrollment Status to reflect the HIT ---
            enrollment.Status = 3; // Status 3: AFIS Hit Detected (Requires Manual Review)
            enrollment.Hit = 1; // 1 = Found Duplicate
            enrollment.AFISPersonHit = personWithHit.PersonB;
            enrollment.AFISDateProcess = personWithHit.DateDetected;

            // Optional: You may want to set the main Citizen's BiometricBypass = true here
            // to temporarily block system access until the hit is resolved.
            // citizen.BiometricBypass = true; 

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Citizen record updated: AFIS Hit Detected. Manual review required.",
                citizenId = citizen.Id,
                duplicateOf = personWithHit.PersonB
            });
        }

        // POST api/AFIS/validated
        /// <summary>
        /// Updates a citizen's biometric enrollment record after AFIS reports no duplicate match (Validated).
        /// </summary>
        [HttpPost("validated")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePersonWithHit([FromBody] PersonWithHitDto personWithHit)
        {
            // Find the LATEST biometric record associated with this citizen
            var enrollment = await _context.BiometricEnrollments
                .Where(b => b.PersonId == personWithHit.PersonA)
                .OrderByDescending(b => b.DateUpload)
                .FirstOrDefaultAsync();

            if (enrollment == null)
            {
                // Return NotFound as the enrollment record is the primary data target
                return NotFound(new { message = $"Active biometric enrollment for Citizen ID {personWithHit.PersonA} not found." });
            }

            // --- Update Biometric Enrollment Status to reflect the HIT ---
            enrollment.Status = 3;
            enrollment.AFISHit = 1;
            enrollment.AFISPersonHit = personWithHit.PersonB;
            enrollment.AFISDateProcess = personWithHit.DateDetected;

            // NOTE: If you need to update citizen.BiometricBypass, you MUST fetch the citizen record.
            // var citizen = await _context.Citizens.FindAsync(enrollment.PersonId);
            // if (citizen != null) citizen.BiometricBypass = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Citizen record updated: AFIS Hit Detected. Manual review required.",
                citizenId = enrollment.PersonId, // Use enrollment.PersonId directly
                duplicateOf = personWithHit.PersonB
            });
        }
    }
}