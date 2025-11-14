
using OnlineRegistration.Server.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRegistration.Server.Data;
using OnlineRegistration.Server.Models;
using OnlineRegistration.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRegistration.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    [Authorize]
    public class KitUserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AfisQueueService _afisQueue;
        public KitUserController(AppDbContext context, AfisQueueService afisQueue)
        {
            _context = context;
            _afisQueue = afisQueue;
        }

        // --- GET: List All Citizens with Latest Biometric Info ---
        [HttpGet("citizen-list")]
        public async Task<ActionResult<IEnumerable<CitizenReadDto>>> GetCitizens()
        {
            // This query is optimized and relies heavily on correct FK mapping in the model.
            var citizens = await _context.Citizens
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    Citizen = c,
                    LatestBiometric = c.BiometricEnrollments
                        .OrderByDescending(b => b.DateUpload)
                        .FirstOrDefault()
                })
                .Select(x => new CitizenReadDto
                {
                    Id = x.Citizen.Id,
                    CitizenType = x.Citizen.CitizenType,
                    FirstName = x.Citizen.FirstName,
                    LastName = x.Citizen.LastName,
                    BirthDate = x.Citizen.BirthDate,
                    CreatedAt = x.Citizen.CreatedAt,
                    BiometricBypass = x.Citizen.BiometricBypass,

                    BiometricId = x.LatestBiometric == null ? (int?)null : x.LatestBiometric.Id,
                    DateCapture = x.LatestBiometric == null ? (DateTime?)null : x.LatestBiometric.DateCapture,
                    Status = x.LatestBiometric == null ? (int?)null : x.LatestBiometric.Status
                })
                .ToListAsync();

            return Ok(citizens);
        }

        // --- GET: Get specific citizen with detailed biometric history (Refactored to use DTOs) ---
        [HttpGet("citizen-list/{id}")]
        public async Task<IActionResult> GetCitizen(int id)
        {
            var citizen = await _context.Citizens
                .Include(c => c.BiometricEnrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (citizen == null)
            {
                return NotFound(new { message = "Citizen not found." });
            }

            var detailDto = new CitizenDetailDto
            {
                Id = citizen.Id,
                CitizenType = citizen.CitizenType,
                FirstName = citizen.FirstName,
                LastName = citizen.LastName,
                BirthDate = citizen.BirthDate,
                CreatedAt = citizen.CreatedAt,
                BiometricBypass = citizen.BiometricBypass,
                BiometricHistory = citizen.BiometricEnrollments
                    .Select(b => new BiometricReadDto
                    {
                        Id = b.Id,
                        // FIX: Map nullable types directly (b.DateCapture is DateTime? in the model)
                        DateCapture = b.DateCapture ?? DateTime.MinValue,
                        DateUpload = b.DateUpload ?? DateTime.MinValue,
                        //DateActivate = b.DateActivate,
                        // Status, Hit, KitUser, KitName added based on prior model fixes
                        Status = b.Status, // BiometricReadDto has Status as non-nullable int
                        Hit = b.Hit, // Assuming BiometricReadDto maps Hit as non-nullable int
                        KitUser = b.KitUser,
                        KitName = b.KitName,

                        Photo = b.Photo,
                        Signature = b.Signature,
                        LeftThumb = b.LeftThumb,
                        LeftIndex = b.LeftIndex,
                        LeftMiddle = b.LeftMiddle,
                        LeftRing = b.LeftRing,
                        LeftSmall = b.LeftSmall,
                        RightThumb = b.RightThumb,
                        RightIndex = b.RightIndex,
                        RightMiddle = b.RightMiddle,
                        RightRing = b.RightRing,
                        RightSmall = b.RightSmall,
                        EyeLeft = b.EyeLeft,
                        EyeRight = b.EyeRight,
                        BiometricLeft = b.BiometricLeft,
                        BiometricRight = b.BiometricRight
                    })
                    .OrderByDescending(b => b.DateUpload)
                    .ToList()
            };

            return Ok(detailDto);
        }

        // --- POST: Create New Citizen and Biometric Enrollment ---
        [HttpPost("create-new-citizen-and-enrollment")]
        public async Task<IActionResult> CreateCitizen([FromBody] CitizenCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var citizen = new Citizen
            {
                CitizenType = dto.CitizenType, // 0: Senior Citizen, 1: need bypassLog
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                CreatedAt = DateTime.UtcNow,
                BiometricBypass = false // Default to false on creation
            };

            _context.Citizens.Add(citizen);
            await _context.SaveChangesAsync(); // Save citizen to get the new ID

            // Biometrics is required for creation via this endpoint
            if (dto.Biometrics == null)
            {
                return BadRequest(new { message = "Biometric data is required for initial enrollment." });
            }

            var biometric = new BiometricDataEnrollment
            {
                PersonId = citizen.Id,
                DateCapture = DateTime.UtcNow,
                DateUpload = DateTime.UtcNow,
                //DateActivate = null,

                // --- MAPPING BIOMETRIC FIELDS ---
                Photo = dto.Biometrics.Photo,
                Signature = dto.Biometrics.Signature,
                LeftThumb = dto.Biometrics.LeftThumb,
                LeftIndex = dto.Biometrics.LeftIndex,
                LeftMiddle = dto.Biometrics.LeftMiddle,
                LeftRing = dto.Biometrics.LeftRing,
                LeftSmall = dto.Biometrics.LeftSmall,
                RightThumb = dto.Biometrics.RightThumb,
                RightIndex = dto.Biometrics.RightIndex,
                RightMiddle = dto.Biometrics.RightMiddle,
                RightRing = dto.Biometrics.RightRing,
                RightSmall = dto.Biometrics.RightSmall,
                EyeLeft = dto.Biometrics.EyeLeft,
                EyeRight = dto.Biometrics.EyeRight,
                BiometricLeft = dto.Biometrics.BiometricLeft,
                BiometricRight = dto.Biometrics.BiometricRight,

                Hit = 0,
                Status = dto.Biometrics.Status ?? 1 // Default status to 1 (Active)
            };

            _context.BiometricEnrollments.Add(biometric);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCitizen), new { id = citizen.Id }, new
            {
                message = "Citizen and Biometric data successfully enrolled.",
                citizenId = citizen.Id,
                biometricId = biometric.Id
            });
        }

        // --- PUT: Update Citizen Personal Details and optionally add a new Biometric Enrollment ---
        //[HttpPut("update-citizen/{id}")]
        //public async Task<IActionResult> UpdateCitizen(int id, [FromBody] CitizenCreateDto dto)
        //{
        //    var citizen = await _context.Citizens
        //        .Include(c => c.BiometricEnrollments)
        //        .FirstOrDefaultAsync(c => c.Id == id);

        //    if (citizen == null)
        //    {
        //        return NotFound(new { message = "Citizen not found." });
        //    }

        //    // --- Update Personal Details ---
        //    citizen.CitizenType = dto.CitizenType;
        //    citizen.FirstName = dto.FirstName;
        //    citizen.LastName = dto.LastName;
        //    citizen.BirthDate = dto.BirthDate;

        //    // Optional: If the PUT request includes new biometric data, create a new enrollment record
        //    if (dto.Biometrics != null)
        //    {
        //        var newBiometric = new BiometricDataEnrollment
        //        {
        //            PersonId = citizen.Id,
        //            DateCapture = DateTime.UtcNow,
        //            DateUpload = DateTime.UtcNow,
        //            //DateActivate = null,

        //            // --- MAPPING BIOMETRIC FIELDS ---
        //            Photo = dto.Biometrics.Photo,
        //            Signature = dto.Biometrics.Signature,
        //            LeftThumb = dto.Biometrics.LeftThumb,
        //            LeftIndex = dto.Biometrics.LeftIndex,
        //            LeftMiddle = dto.Biometrics.LeftMiddle,
        //            LeftRing = dto.Biometrics.LeftRing,
        //            LeftSmall = dto.Biometrics.LeftSmall,
        //            RightThumb = dto.Biometrics.RightThumb,
        //            RightIndex = dto.Biometrics.RightIndex,
        //            RightMiddle = dto.Biometrics.RightMiddle,
        //            RightRing = dto.Biometrics.RightRing,
        //            RightSmall = dto.Biometrics.RightSmall,
        //            EyeLeft = dto.Biometrics.EyeLeft,
        //            EyeRight = dto.Biometrics.EyeRight,
        //            BiometricLeft = dto.Biometrics.BiometricLeft,
        //            BiometricRight = dto.Biometrics.BiometricRight,

        //            Hit = 0, // Default value
        //            Status = dto.Biometrics.Status ?? 1 // Default status to 1 if null
        //        };
        //        _context.BiometricEnrollments.Add(newBiometric);
        //    }

        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        [HttpPut("set-bypass-status/{id}")]
        public async Task<IActionResult> SetBiometricBypassStatusAndLog(int id, [FromBody] BypassLogDto dto)
        {
            // Input validation: ReasonCode is mandatory when setting bypass (ShouldBypass == true)
            if (dto.ShouldBypass && string.IsNullOrWhiteSpace(dto.ReasonCode))
            {
                return BadRequest(new { message = "ReasonCode is required when setting the biometric bypass status." });
            }

            var citizen = await _context.Citizens.FindAsync(id);

            if (citizen == null)
            {
                return NotFound(new { message = "Citizen not found." });
            }

            if (dto.ShouldBypass)
            {
                // 1. Set Bypass Status and Log
                if (citizen.BiometricBypass)
                {
                    return Ok(new { message = "Biometric bypass is already set for this citizen." });
                }

                citizen.BiometricBypass = true;

                // Create the log entry
                var bypassLog = new BypassLog
                {
                    //PersonId = citizen.Id,
                    StepName = dto.StepName ?? "Step Name",
                    ReasonCode = dto.ReasonCode,
                    ReasonDetails = dto.ReasonDetails,
                    DateBypassed = DateTime.UtcNow
                };

                _context.BypassLogs.Add(bypassLog);
            }
            else
            {
                // 2. Remove Bypass Status
                if (!citizen.BiometricBypass)
                {
                    return Ok(new { message = "Biometric bypass is already disabled for this citizen." });
                }

                citizen.BiometricBypass = false;
                // Optional: You could log the removal event here if needed, but typically only the bypass creation is logged in detail.
            }

            // Save changes to the Citizen record and the new BypassLog (if setting bypass)
            await _context.SaveChangesAsync();

            string statusMessage = dto.ShouldBypass
                ? $"Biometric enrollment for Citizen {id} was successfully set to **Bypassed** due to reason code: {dto.ReasonCode}."
                : $"Biometric enrollment for Citizen {id} was successfully reverted to **Standard** (Bypass removed).";

            return Ok(new
            {
                message = statusMessage,
                citizenId = citizen.Id,
                newBypassStatus = citizen.BiometricBypass
            });
        }

        [HttpPut("applicant/biometrics-lock")]
        public async Task<ActionResult> FinalizeCapture(BiometricDataEnrollmentDto dto)
        {
            // Find the latest biometric enrollment record for the given PersonId 
            // that is currently in a 'pending' status (Status == 1).
            var enrollment = await _context.BiometricEnrollments
                .Where(b => b.PersonId == dto.PersonId && b.Status == 1)
                .OrderByDescending(b => b.DateUpload)
                .FirstOrDefaultAsync();

            if (enrollment == null)
                return NotFound(new { message = $"Pending biometric enrollment not found for Person ID {dto.PersonId}." });

            // Get Philippine Standard Time (UTC+8).
            var phTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Philippine Standard Time");

            // 1. Update the enrollment record status and date.
            enrollment.DateCapture = TimeZoneInfo.ConvertTime(DateTime.UtcNow, phTimeZone);
            enrollment.Status = 2; // Set to 'Completed' or 'Locked'

            await _context.SaveChangesAsync();

            // ✅ Queue all available fingerprints for AFIS using the data stored in the DB record
            var fingerprints = new Dictionary<int, string?>
    {
        { 1, enrollment.LeftThumb },
        { 2, enrollment.LeftIndex },
        { 3, enrollment.LeftMiddle },
        { 4, enrollment.LeftRing },
        { 5, enrollment.LeftSmall },
        { 6, enrollment.RightThumb },
        { 7, enrollment.RightIndex },
        { 8, enrollment.RightMiddle },
        { 9, enrollment.RightRing },
        { 10, enrollment.RightSmall }
    };

            foreach (var fp in fingerprints)
            {
                if (!string.IsNullOrEmpty(fp.Value))
                {
                    // Assuming _afisQueue is correctly injected and AfisJob is defined
                    _afisQueue.Enqueue(new OnlineRegistration.Server.Models.AfisJob
                    {
                        PersonId = enrollment.PersonId, // Use the PersonId from the enrollment model
                        FingerPosition = fp.Key,
                        Base64Image = fp.Value!
                    });
                }
            }

            return Ok(new
            {
                message = $"Biometrics locked for Person {enrollment.PersonId} and enqueued for AFIS processing.",
                enrollmentId = enrollment.Id
            });
        }
    }
}