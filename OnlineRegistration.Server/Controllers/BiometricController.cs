using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRegistration.Server.Data;
using OnlineRegistration.Server.DTOs;
using OnlineRegistration.Server.Models;
using OnlineRegistration.Server.Services.Interfaces;

namespace OnlineRegistration.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BiometricController : ControllerBase
    {
        private readonly EmployeesDbContext _db;
        private readonly IConfiguration _config;
        private readonly IEmailQueue _emailQueue;
        public BiometricController(EmployeesDbContext db, IConfiguration config, IEmailQueue emailQueue)
        {
            _db = db;
            _config = config;
            _emailQueue = emailQueue;

        }
        // GET: api/<BiometricController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        // PUT api/<BiometricController>/5
        [HttpPost]
        public async Task<ActionResult> PostAttendance([FromBody] EmployeeAttendanceDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid attendance data.");
            var attendance = new EmployeeAttendance {
                EmployeeID = dto.EmployeeID,
                BiometricDeviceID = dto.BiometricDeviceID,
                Mode = dto.Mode,
                Data = dto.Data,
                DateLog = dto.DateLog,
                EventType = dto.EventType,
            };

            _db.Attendance.Add(attendance);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Biometrics uploaded successfully" });
        }
    }
}
