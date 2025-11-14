//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OnlineRegistration.Server.Data;
//using OnlineRegistration.Server.DTOs;
//using OnlineRegistration.Server.Models;
//using OnlineRegistration.Server.Services.Interfaces;

//namespace OnlineRegistration.Server.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class BiometricController : ControllerBase
//    {
//        private readonly EmployeesDbContext _db;
//        private readonly IConfiguration _config;
//        private readonly IEmailQueue _emailQueue;
//        public BiometricController(EmployeesDbContext db, IConfiguration config, IEmailQueue emailQueue)
//        {
//            _db = db;
//            _config = config;
//            _emailQueue = emailQueue;

//        }
//        // GET: api/<BiometricController>
//        [HttpGet]
//        public IEnumerable<string> Get()
//        {
//            return new string[] { "value1", "value2" };
//        }



//        // PUT api/<BiometricController>/5
//        [HttpPost]
//        public async Task<ActionResult> PostAttendance([FromBody] EmployeeAttendanceDto dto)
//        {
//            if (dto == null)
//                return BadRequest("Invalid attendance data.");
//            var attendance = new EmployeeAttendance {
//                EmployeeID = dto.EmployeeID,
//                BiometricDeviceID = dto.BiometricDeviceID,
//                Mode = dto.Mode,
//                Data = dto.Data,
//                DateLog = dto.DateLog,
//                EventType = dto.EventType,
//            };

//            _db.Attendance.Add(attendance);
//            await _db.SaveChangesAsync();
//            return Ok(new { message = "Biometrics uploaded successfully" });
//        }
//    }
//}



using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRegistration.Server.Data;
using OnlineRegistration.Server.DTOs;
using OnlineRegistration.Server.Models;
using OnlineRegistration.Server.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace OnlineRegistration.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailQueue _emailQueue;

        public VerificationController(AppDbContext db, IConfiguration config, IEmailQueue emailQueue)
        {
            _context = db;
            _config = config;
            _emailQueue = emailQueue;
        }

        // GET: api/Verification
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public ActionResult<IEnumerable<string>> Get()
        {
            return Ok(new string[] { "Verification Controller", "Biometric Logging Endpoint Ready" });
        }



        /// Logs a biometric verification event for a Citizen.
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> PostVerificationLog([FromBody] CitizenVerificationDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid verification data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var citizenExists = await _context.Citizens.AnyAsync(c => c.Id == dto.PersonId);
            if (!citizenExists)
            {
                return NotFound(new { message = $"Citizen with ID {dto.PersonId} not found." });
            }

            var verificationLog = new CitizenVerificationLog
            {
                PersonId = dto.PersonId,
                BiometricDeviceID = dto.BiometricDeviceID,
                Mode = dto.Mode,
                EventType = dto.EventType,
                DateLog = dto.DateLog,
                VerificationData = dto.VerificationData,
                DateCreated = DateTime.UtcNow
            };

            try
            {
                _context.VerificationLogs.Add(verificationLog);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Biometric verification log recorded successfully.",
                    logId = verificationLog.Id
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the verification log.", details = ex.Message });
            }
        }
    }
}
