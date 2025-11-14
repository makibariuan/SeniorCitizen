//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OnlineRegistration.Server.Data;
//using OnlineRegistration.Server.DTOs;
//using OnlineRegistration.Server.Models;
//using OnlineRegistration.Server.Services;

//namespace OnlineRegistration.Server.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class KitController : ControllerBase
//    {
//        private readonly EmployeesDbContext _db;
//        private readonly AfisQueueService _afisQueue;

//        public KitController(EmployeesDbContext db, AfisQueueService afisQueue)
//        {
//            _db = db;
//            _afisQueue = afisQueue;
//        }

//        [HttpGet("applicant/{id}")]
//        public async Task<ActionResult<EmployeeIDApplicationDto>> GetApplicantDetails(string id)
//        {
//            var application = await _db.EmployeeIDApplications
//                .Where(p => p.ApplicationCode == id && p.Status == 1)
//                .AsNoTracking()
//                .FirstOrDefaultAsync();

//            if (application == null)
//                return NotFound();

//            var person = await _db.PersonalInformations
//                .Include(p => p.Children)
//                .Include(p => p.EducationRecords)
//                .Include(p => p.VoluntaryWorks)
//                .Include(p => p.Trainings)
//                .Include(p => p.SkillsHobbies)
//                .Include(p => p.Distinctions)
//                .Include(p => p.Memberships)
//                .Include(p => p.CivilServiceEligibilities)
//                .Include(p => p.WorkExperiences)
//                .AsNoTracking()
//                .FirstOrDefaultAsync(p => p.PersonID == application.PersonID);

//            if (person == null)
//                return NotFound();

//            // Map to DTO
//            var dto = new EmployeeIDApplicationDto
//            {
//                PersonID = application.PersonID,
//                ApplicationCode = application.ApplicationCode,
//                Surname = application.Surname,
//                FirstName = application.FirstName,
//                MiddleName = application.MiddleName,
//                DateOfBirth = application.DateOfBirth?.ToString("MM/dd/yyyy"),
//                EmployeeID = application.EmployeeID,
//                DepartmentID = application.DepartmentID,
//                DepartmentName = application.DepartmentName,
//                Designation = application.Designation,
//                PrintDesignation = application.PrintDesignation,
//                DateSchedule = application.DateSchedule
//            };

//            return Ok(dto);
//        }

//        [HttpPut("applicant/biometrics")]
//        public async Task<ActionResult> UploadApplicantBiometrics(EmployeeIDBiometricsDto dto)
//        {
//            var application = await _db.EmployeeIDApplications
//                .Where(p => p.ApplicationCode == dto.ApplicationCode && p.Status == 1)
//                .FirstOrDefaultAsync();

//            if (application == null)
//                return NotFound();

//            // Update only fields that have data
//            if (dto.DateCapture.HasValue) application.DateCapture = dto.DateCapture.Value;
//            if (!string.IsNullOrEmpty(dto.Photo)) application.Photo = dto.Photo;
//            if (!string.IsNullOrEmpty(dto.Signature)) application.Signature = dto.Signature;

//            if (!string.IsNullOrEmpty(dto.LeftThumb)) application.LeftThumb = dto.LeftThumb;
//            if (!string.IsNullOrEmpty(dto.LeftIndex)) application.LeftIndex = dto.LeftIndex;
//            if (!string.IsNullOrEmpty(dto.LeftMiddle)) application.LeftMiddle = dto.LeftMiddle;
//            if (!string.IsNullOrEmpty(dto.LeftRing)) application.LeftRing = dto.LeftRing;
//            if (!string.IsNullOrEmpty(dto.LeftSmall)) application.LeftSmall = dto.LeftSmall;

//            if (!string.IsNullOrEmpty(dto.RightThumb)) application.RightThumb = dto.RightThumb;
//            if (!string.IsNullOrEmpty(dto.RightIndex)) application.RightIndex = dto.RightIndex;
//            if (!string.IsNullOrEmpty(dto.RightMiddle)) application.RightMiddle = dto.RightMiddle;
//            if (!string.IsNullOrEmpty(dto.RightRing)) application.RightRing = dto.RightRing;
//            if (!string.IsNullOrEmpty(dto.RightSmall)) application.RightSmall = dto.RightSmall;

//            if (!string.IsNullOrEmpty(dto.EyeLeft)) application.EyeLeft = dto.EyeLeft;
//            if (!string.IsNullOrEmpty(dto.EyeRight)) application.EyeRight = dto.EyeRight;
//            if (!string.IsNullOrEmpty(dto.BiometricLeft)) application.BiometricLeft = dto.BiometricLeft;
//            if (!string.IsNullOrEmpty(dto.BiometricRight)) application.BiometricRight = dto.BiometricRight;

//            await _db.SaveChangesAsync();
//            return Ok(new { message = "Biometrics updated successfully" });
//        }

//        [HttpPut("applicant/biometrics-lock")]
//        public async Task<ActionResult> FinalizeCapture(EmployeeIDBiometricsDto dto)
//        {
//            var application = await _db.EmployeeIDApplications
//                .Where(p => p.ApplicationCode == dto.ApplicationCode && p.Status == 1)
//                .FirstOrDefaultAsync();

//            if (application == null)
//                return NotFound();

//            // Get Philippine Standard Time (UTC+8)
//            var phTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
//            application.DateCapture = TimeZoneInfo.ConvertTime(DateTime.UtcNow, phTimeZone);
//            application.Status = 2; // completed

//            await _db.SaveChangesAsync();

//            // ✅ Queue all available fingerprints for AFIS
//            var fingerprints = new Dictionary<int, string?>
//            {
//                { 1, application.LeftThumb },
//                { 2, application.LeftIndex },
//                { 3, application.LeftMiddle },
//                { 4, application.LeftRing },
//                { 5, application.LeftSmall },
//                { 6, application.RightThumb },
//                { 7, application.RightIndex },
//                { 8, application.RightMiddle },
//                { 9, application.RightRing },
//                { 10, application.RightSmall }
//            };

//            foreach (var fp in fingerprints)
//            {
//                if (!string.IsNullOrEmpty(fp.Value))
//                {
//                    _afisQueue.Enqueue(new AfisJob
//                    {
//                        PersonId = application.PersonID,
//                        FingerPosition = fp.Key,
//                        Base64Image = fp.Value!
//                    });
//                }
//            }

//            return Ok(new { message = "Biometrics locked and enqueued for AFIS processing." });
//        }
//    }
//}
