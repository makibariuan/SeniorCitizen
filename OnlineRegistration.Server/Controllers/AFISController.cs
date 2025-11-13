using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRegistration.Server.Data;
using OnlineRegistration.Server.Models;
using OnlineRegistration.Server.Services.Interfaces;
using OnlineRegistration.Server.DTOs;
using System.Text.Json;

namespace OnlineRegistration.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AFISController : ControllerBase
    {
        private readonly EmployeesDbContext _db;
        private readonly IConfiguration _config;

        public AFISController(EmployeesDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;

        }

        [HttpPost("hit")]
        public async Task<IActionResult> UpdatePersonWithHit([FromBody] PersonWithHitDto personWithHit)
        {
            var person = await _db.EmployeeIDApplications
                .FirstOrDefaultAsync(p => p.PersonID == personWithHit.PersonA);

            if (person == null)
            {
                return BadRequest(new { message = "Person not found" });
            }

            person.Status = 3;
            person.AFISHit = 1; // found duplicate
            person.AFISPersonHit = personWithHit.PersonB;
            person.AFISDateProcess = personWithHit.DateDetected;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Record updated" });
        }

        [HttpPost("validated")]
        public async Task<IActionResult> UpdatePersonNoHit([FromBody] PersonValidatedDto personValidated)
        {
            var person = await _db.EmployeeIDApplications
                .FirstOrDefaultAsync(p => p.PersonID == personValidated.PersonId);

            if (person == null)
            {
                return BadRequest(new { message = "Person not found" });
            }

            if(person.AFISPersonHit != null)
                return Ok(new { message = "Record not updated due to hit" });

            person.Status = 4;
            person.AFISHit = 0; // found duplicate
            person.AFISDateProcess = personValidated.ValidatedAt;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Record updated" });
        }

    }
}
