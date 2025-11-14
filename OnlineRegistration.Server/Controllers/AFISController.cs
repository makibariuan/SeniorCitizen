

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OnlineRegistration.Server.Data; // Use AppDbContext
//using OnlineRegistration.Server.Models;
//using OnlineRegistration.Server.DTOs;
//using System.Threading.Tasks;

//namespace OnlineRegistration.Server.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AFISController : ControllerBase
//    {
//        // Changed to AppDbContext for citizen data
//        private readonly AppDbContext _context;
//        private readonly IConfiguration _config;

//        public AFISController(AppDbContext context, IConfiguration config)
//        {
//            _context = context; // Renamed to _context for convention
//            _config = config;
//        }

//        // POST api/AFIS/hit
//        /// <summary>
//        /// Updates a citizen's biometric enrollment record after AFIS reports a duplicate match (Hit).
//        /// </summary>
//        [HttpPost("hit")]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(404)]
//        public async Task<IActionResult> UpdatePersonWithHit([FromBody] PersonWithHitDto personWithHit)
//        {
//            // Find the citizen record using the PersonID (PersonA in the AFIS payload)
//            var citizen = await _context.Citizens
//                .FirstOrDefaultAsync(c => c.Id == personWithHit.PersonA);

//            if (citizen == null)
//            {
//                return NotFound(new { message = $"Citizen with ID {personWithHit.PersonA} not found." });
//            }

//            // Find the LATEST (Status=2/Locked) biometric record associated with this citizen
//            var enrollment = await _context.BiometricEnrollments
//                .Where(b => b.PersonId == personWithHit.PersonA)
//                .OrderByDescending(b => b.DateUpload)
//                .FirstOrDefaultAsync();

//            if (enrollment == null)
//            {
//                // Handle case where citizen exists but enrollment record is missing
//                return NotFound(new { message = $"Active biometric enrollment for Citizen ID {personWithHit.PersonA} not found." });
//            }

//            // --- Update Biometric Enrollment Status to reflect the HIT ---
//            enrollment.Status = 3; // Status 3: AFIS Hit Detected (Requires Manual Review)
//            enrollment.Hit = 1; // 1 = Found Duplicate
//            enrollment.AFISPersonHit = personWithHit.PersonB;
//            enrollment.AFISDateProcess = personWithHit.DateDetected;

//            // Optional: You may want to set the main Citizen's BiometricBypass = true here
//            // to temporarily block system access until the hit is resolved.
//            // citizen.BiometricBypass = true; 

//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                message = "Citizen record updated: AFIS Hit Detected. Manual review required.",
//                citizenId = citizen.Id,
//                duplicateOf = personWithHit.PersonB
//            });
//        }

//        // POST api/AFIS/validated
//        /// <summary>
//        /// Updates a citizen's biometric enrollment record after AFIS reports no duplicate match (Validated).
//        /// </summary>
//        [HttpPost("validated")]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(404)]
//        public async Task<IActionResult> UpdatePersonNoHit([FromBody] PersonValidatedDto personValidated)
//        {
//            // Find the LATEST (Status=2/Locked) biometric record associated with this citizen
//            var enrollment = await _context.BiometricEnrollments
//                .Where(b => b.PersonId == personValidated.PersonId)
//                .OrderByDescending(b => b.DateUpload)
//                .FirstOrDefaultAsync();

//            if (enrollment == null)
//            {
//                return NotFound(new { message = $"Active biometric enrollment for Citizen ID {personValidated.PersonId} not found." });
//            }

//            // Safety Check: If a Hit was recorded previously (Status=3) and this is called out of order, 
//            // we should not overwrite the hit status.
//            if (enrollment.Hit == 1)
//            {
//                // This means the 'hit' endpoint was called first. We ignore the 'validated' status.
//                return Ok(new { message = "Record skipped: AFIS hit was previously detected." });
//            }

//            // --- Update Biometric Enrollment Status to reflect No Hit ---
//            enrollment.Status = 4; // Status 4: AFIS Validated (Ready for Activation)
//            enrollment.Hit = 0; // 0 = No Duplicate Found
//            enrollment.AFISDateProcess = personValidated.ValidatedAt;

//            // Optional: You could now activate the Citizen's profile here
//            // enrollment.DateActivate = DateTime.UtcNow;

//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                message = "Citizen record updated: AFIS Validation Successful (No Hit).",
//                citizenId = enrollment.PersonId
//            });
//        }
//    }
//}