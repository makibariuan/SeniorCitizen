using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRegistration.Server.Data;
using OnlineRegistration.Server.DTOs;
using OnlineRegistration.Server.Models;
using OnlineRegistration.Server.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace OnlineRegistration.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeesDbContext _db;
        private readonly IConfiguration _config;
        private readonly IEmailQueue _emailQueue;

        public EmployeeController(EmployeesDbContext db, IConfiguration config, IEmailQueue emailQueue)
        {
            _db = db;
            _config = config;
            _emailQueue = emailQueue;

        }

        // ---------------- Lookup Tables ----------------
        [HttpGet("gender")]
        public async Task<ActionResult<IEnumerable<Gender>>> GetGenderList()
        {
            var genders = await _db.Gender.AsNoTracking().ToListAsync();
            return Ok(genders);
        }

        [HttpGet("civilstatus")]
        public async Task<ActionResult<IEnumerable<CivilStatus>>> GetCivilStatusList()
        {
            var statuses = await _db.CivilStatus.AsNoTracking().ToListAsync();
            return Ok(statuses);
        }

        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartmentList()
        {
            var dts = await _db.Department.AsNoTracking().ToListAsync();
            return Ok(dts);
        }


        [HttpGet("schedule")]
        public async Task<ActionResult<IEnumerable<EmployeeIDApplication>>> GetBiomentricsSchedule()
        {
            var applications = await _db.EmployeeIDApplications
                .AsNoTracking()
                .Where(p => p.DateSchedule == null && p.Status == 0)
                .ToListAsync();
            return Ok(applications);
        }

        [HttpGet("attendance/{id}")]
        public async Task<ActionResult> GetAttendance(string id)
        {
            var data = await _db.Attendance
                .Include(a => a.BiometricDevice)
                    .ThenInclude(d => d.Department)
                .AsNoTracking()
                .Where(a => a.EmployeeID == id)
                .Select(a => new
                {
                    a.EmployeeID,
                    a.DateLog,
                    a.Mode,
                    a.BiometricDeviceID,
                    a.EventType,
                    DepartmentName = a.BiometricDevice.Department.DepartmentName
                })
                .OrderByDescending(a => a.DateLog)
                .ToListAsync();

            // ✅ Split DateLog in memory (EF can’t translate custom ToString formats)
            var result = data.Select(a => new
            {
                a.EmployeeID,
                Date = a.DateLog.ToString("yyyy-MM-dd"),
                Time = a.DateLog.ToString("HH:mm:ss"),
                Mode = a.Mode == 1 ? "IN" : a.Mode == 2 ? "OUT" : "UNKNOWN",
                a.BiometricDeviceID,
                a.EventType,
                a.DepartmentName
            });

            return Ok(result);
        }



        [HttpGet("print-cards")]
        public async Task<ActionResult<IEnumerable<EmployeeIDPrintDto>>> GetBiomentricsForPrinting()
        {
            var applications = await _db.EmployeeIDApplications
                .AsNoTracking()
                .Where(p => p.Status == 2)
                .Select(p => new
                {
                    p.PersonID,
                    p.ApplicationCode,
                    p.DepartmentName,
                    FullName = p.FirstName + " " + p.MiddleName + " " + p.Surname,
                    p.DateCapture,
                    p.Photo
                })
                .ToListAsync();

            // Now safely format and map to DTO
            var result = applications.Select(a => new EmployeeIDPrintDto
            {
                PersonID = a.PersonID,
                ApplicationCode = a.ApplicationCode,
                Department = a.DepartmentName,
                FullName = a.FullName,
                DateCapture = a.DateCapture.HasValue ? a.DateCapture.Value.ToString("MM/dd/yyyy") : "",
                Photo = a.Photo != null ? a.Photo : ""
            });

            return Ok(result);
        }

        [HttpPost("print-cards/csv")]
        public async Task<IActionResult> ExportSelectedCardsToCsv([FromBody] List<string> selectedCodes)
        {
            if (selectedCodes == null || !selectedCodes.Any())
                return BadRequest("No application codes selected.");

            // Query only the selected applications
            var selectedApps = await _db.EmployeeIDApplications
                .Where(p => selectedCodes.Contains(p.ApplicationCode))
                .ToListAsync();

            if (!selectedApps.Any())
                return NotFound("No matching applications found.");

            // --- Update Status to 3 ---
            // For Printing
            foreach (var app in selectedApps)
            {
                app.Status = 3;
            }

            await _db.SaveChangesAsync();

            // --- Rebuild lightweight list for CSV export ---
            var exportList = selectedApps
                .Select(p => new
                {
                    p.ApplicationCode,
                    FullName = p.FirstName + " " + (string.IsNullOrEmpty(p.MiddleName) ? "" : p.MiddleName + " ") + p.Surname,
                    Department = p.DepartmentName,
                    p.Photo
                })
                .ToList();

            // Build CSV content
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ApplicationCode,FullName,Department,Photo");

            foreach (var app in exportList)
            {
                string safeName = $"\"{app.FullName.Replace("\"", "\"\"")}\"";
                string safeDept = $"\"{app.Department?.Replace("\"", "\"\"")}\"";
                string safePhoto = app.Photo != null ? app.Photo : "";
                sb.AppendLine($"{app.ApplicationCode},{safeName},{safeDept},{safePhoto}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "IDPrintFile.csv");
        }



        [HttpPost("schedule/update")]
        public async Task<IActionResult> UpdateSchedule([FromBody] ScheduleUpdateRequest request)
        {
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 80,
                    Margin = 2
                },
                Renderer = new PixelDataRenderer()
            };

            string barcodeFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "barcodes");
            if (!Directory.Exists(barcodeFolder))
                Directory.CreateDirectory(barcodeFolder);

            foreach (var id in request.PersonIDs)
            {
                var app = await _db.EmployeeIDApplications.FindAsync(id);
                if (app != null)
                {
                    app.DateSchedule = request.Date;
                    app.Status = 1;
                    app.ApplicationCode = GenerateApplicationCode();
                }

                var user = await _db.PersonalInformations.FirstOrDefaultAsync(u => u.PersonID == id);
                if (user != null && app != null)
                {
                    // Generate ZXing PixelData
                    var pixelData = barcodeWriter.Write(app.ApplicationCode);

                    // Convert PixelData to ImageSharp Image<Rgba32>
                    using var image = new Image<Rgba32>(pixelData.Width, pixelData.Height);
                    for (int y = 0; y < pixelData.Height; y++)
                    {
                        for (int x = 0; x < pixelData.Width; x++)
                        {
                            int idx = (y * pixelData.Width + x) * 4;
                            image[x, y] = new Rgba32(
                                pixelData.Pixels[idx],
                                pixelData.Pixels[idx + 1],
                                pixelData.Pixels[idx + 2],
                                pixelData.Pixels[idx + 3]);
                        }
                    }

                    // Save PNG to wwwroot/barcodes
                    string fileName = $"{app.ApplicationCode}.png";
                    string filePath = Path.Combine(barcodeFolder, fileName);
                    image.Save(filePath, new PngEncoder());

                    // Generate public URL
                    var domain = _config["BarcodeSettings:PublicBaseUrl"];
                    string publicUrl = $"{domain}/{fileName}";

                    // Build HTML email with public URL
                    string emailBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; color: #000000; background-color: #FFFFFF; padding: 20px;'>
                            <p>Hello {user.FirstName},</p>
                            <p>Your application schedule is on {app.DateSchedule:MMMM dd, yyyy}.</p>
                            <p>Please present the application number below:</p>
                            <img src='{publicUrl}' alt='Barcode' style='display:block; margin-top:10px;'/>
                            <p style='font-size:12px; color:#555555;'>
                                If the barcode is not visible, please show this code as plain text: {app.ApplicationCode}
                            </p>
                        </body>
                        </html>";

                    _emailQueue.QueueEmail(new EmailMessage
                    {
                        To = user.Email,
                        Subject = "Schedule of Biometric Capture for Employee ID",
                        Body = emailBody
                    });
                }
            }

            await _db.SaveChangesAsync();
            return Ok(new { message = "Schedule updated successfully" });
        }

        [HttpGet("id-stats")]
        public async Task<IActionResult> GetIDStats()
        {
            var stats = await _db.EmployeeIDApplications
                .GroupBy(e => 1)
                .Select(g => new
                {
                    EmployeeCount = g.Count(),
                    ForSchedule = g.Count(e => e.Status == 0),
                    Scheduled = g.Count(e => e.Status == 1),
                    Captured = g.Count(e => e.Status == 2),
                    ForPrinting = g.Count(e => e.Status == 3),
                    ActiveCards = g.Count(e => e.Status == 4),
                })
                .FirstOrDefaultAsync()
                ?? new { EmployeeCount = 0, ForSchedule = 0, Scheduled=1, Captured = 0, ForPrinting = 0, ActiveCards = 0 };

            return Ok(stats);
        }

        private string GenerateApplicationCode()
        {
            // Example: MKT-20251011-ABC123
            return $"MKT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }

        public class ScheduleUpdateRequest
        {
            public DateTime Date { get; set; }
            public List<int> PersonIDs { get; set; }
        }


        [HttpGet("user/{id}")]
        public async Task<ActionResult<PersonalInformationDto>> GetC1(int id)
        {
            var person = await _db.PersonalInformations
                .Include(p => p.Children)
                .Include(p => p.EducationRecords)
                .Include(p => p.VoluntaryWorks)
                .Include(p => p.Trainings)
                .Include(p => p.SkillsHobbies)
                .Include(p => p.Distinctions)
                .Include(p => p.Memberships)
                .Include(p => p.CivilServiceEligibilities)
                .Include(p => p.WorkExperiences)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PersonID == id);

            if (person == null) return NotFound();

            // Map to DTO
            var dto = new PersonalInformationDto
            {
                PersonID = person.PersonID,
                CsIdNo = person.CsIdNo,
                Surname = person.Surname,
                FirstName = person.FirstName,
                MiddleName = person.MiddleName,
                NameExtension = person.NameExtension,
                DateOfBirth = person.DateOfBirth,
                PlaceOfBirth = person.PlaceOfBirth,
                SexID = person.SexID,
                CivilStatusID = person.CivilStatusID,
                Citizenship = person.Citizenship,
                HeightCM = person.HeightCM,
                WeightKG = person.WeightKG,
                BloodType = person.BloodType,
                GsisID = person.GsisID,
                PagibigID = person.PagibigID,
                PhilhealthID = person.PhilhealthID,
                SssNo = person.SssNo,
                Tin = person.Tin,
                AgencyEmployeeNo = person.AgencyEmployeeNo,
                ResidentialAddress = person.ResidentialAddress,
                ResidentialZip = person.ResidentialZip,
                PermanentAddress = person.PermanentAddress,
                PermanentZip = person.PermanentZip,
                TelephoneNo = person.TelephoneNo,
                MobileNo = person.MobileNo,
                Email = person.Email,
                SpouseSurname = person.SpouseSurname,
                SpouseFirstName = person.SpouseFirstName,
                SpouseMiddleName = person.SpouseMiddleName,
                SpouseNameExtension = person.SpouseNameExtension,
                SpouseOccupation = person.SpouseOccupation,
                SpouseEmployer = person.SpouseEmployer,
                SpouseBusinessAddress = person.SpouseBusinessAddress,
                SpouseTelephone = person.SpouseTelephone,
                FatherSurname = person.FatherSurname,
                FatherFirstName = person.FatherFirstName,
                FatherMiddleName = person.FatherMiddleName,
                FatherNameExtension = person.FatherNameExtension,
                MotherSurname = person.MotherSurname,
                MotherFirstName = person.MotherFirstName,
                MotherMiddleName = person.MotherMiddleName,

                DepartmentID = person.DepartmentID,
                Designation = person.Designation,

                Children = person.Children.Select(c => new ChildDto
                {
                    ChildID = c.ChildId,
                    PersonID = c.PersonID,
                    FullName = c.FullName,
                    DateOfBirth = c.DateOfBirth
                }).ToList(),
                EducationRecords = person.EducationRecords.Select(e => new EducationDto
                {
                    Level = e.Level,
                    SchoolName = e.SchoolName,
                    Degree = e.Degree,
                    AttendanceFrom = e.AttendanceFrom,
                    AttendanceTo = e.AttendanceTo,
                    HighestLevel = e.HighestLevel,
                    YearGraduated = e.YearGraduated,
                    Honors = e.Honors
                }).ToList(),
                VoluntaryWorks = person.VoluntaryWorks.Select(v => new VoluntaryWorkDTO
                {
                    WorkId = v.WorkId,
                    PersonID = v.PersonID,
                    Organization = v.Organization,
                    DateFrom = v.DateFrom,
                    DateTo = v.DateTo,
                    NumberOfHours = v.NumberOfHours,
                    Position = v.Position
                }).ToList(),
                Trainings = person.Trainings.Select(t => new TrainingDTO
                {
                    TrainingId = t.TrainingId,
                    PersonID = t.PersonID,
                    Title = t.Title,
                    DateFrom = t.DateFrom,
                    DateTo = t.DateTo,
                    NumberOfHours = t.NumberOfHours,
                    TypeOfLD = t.TypeOfLD,
                    ConductedBy = t.ConductedBy
                }).ToList(),
                SkillsHobbies = person.SkillsHobbies.Select(s => new SkillHobbyDTO
                {
                    SkillId = s.SkillId,
                    PersonID = s.PersonID,
                    SkillOrHobby = s.SkillOrHobby
                }).ToList(),
                Distinctions = person.Distinctions.Select(d => new DistinctionDTO
                {
                    DistinctionId = d.DistinctionId,
                    PersonID = d.PersonID,
                    Distinction = d.DistinctionName
                }).ToList(),
                Memberships = person.Memberships.Select(m => new MembershipDTO
                {
                    MembershipId = m.MembershipId,
                    PersonID = m.PersonID,
                    OrganizationName = m.OrganizationName
                }).ToList(),
                CivilServiceEligibilities = person.CivilServiceEligibilities.Select(e => new CivilServiceEligibilityDTO
                {
                    EligibilityID = e.EligibilityID,
                    PersonID = e.PersonID,
                    CareerService = e.CareerService,
                    Rating = e.Rating,
                    DateOfExamination = e.DateOfExamination,
                    PlaceOfExamination = e.PlaceOfExamination,
                    LicenseNumber = e.LicenseNumber,
                    LicenseValidity = e.LicenseValidity,
                    DateReleased = e.DateReleased
                }).ToList(),
                WorkExperiences = person.WorkExperiences.Select(w => new WorkExperienceDTO
                {
                    WorkExperienceID = w.WorkExperienceId,
                    PersonID = w.PersonID,
                    DateFrom = w.DateFrom,
                    DateTo = w.DateTo,
                    PositionTitle = w.PositionTitle,
                    DepartmentAgencyCompany = w.DepartmentAgencyCompany,
                    MonthlySalary = w.MonthlySalary,
                    SalaryGradeStep = w.SalaryGradeStep,
                    StatusOfAppointment = w.StatusOfAppointment,
                    IsGovernmentService = w.IsGovernmentService,
                    Description = w.Description
                }).ToList(),
                //Department = person.Department, // <-- Map Department model
                //Designation = person.Designation // <-- Map Designation string
            };

            return Ok(dto);
        }

        [HttpPut("user/{id}")]
        public async Task<IActionResult> UpsertC1(int id, PersonalInformationDto dto)
        {
            var person = await _db.PersonalInformations
                .Include(p => p.Children)
                .Include(p => p.EducationRecords)
                .Include(p => p.VoluntaryWorks)
                .Include(p => p.Trainings)
                .Include(p => p.SkillsHobbies)
                .Include(p => p.Distinctions)
                .Include(p => p.Memberships)
                .Include(p => p.CivilServiceEligibilities)
                .Include(p => p.WorkExperiences)
                .FirstOrDefaultAsync(p => p.PersonID == id);

            if (person == null)
            {
                person = new PersonalInformation();
                _db.PersonalInformations.Add(person);
            }

            // Map simple fields
            person.PersonID = id;
            person.CsIdNo = dto.CsIdNo;
            person.Surname = dto.Surname;
            person.FirstName = dto.FirstName;
            person.MiddleName = dto.MiddleName;
            person.NameExtension = dto.NameExtension;
            person.DateOfBirth = dto.DateOfBirth;
            person.PlaceOfBirth = dto.PlaceOfBirth;
            person.SexID = dto.SexID;
            person.CivilStatusID = dto.CivilStatusID;
            person.Citizenship = dto.Citizenship;
            person.HeightCM = dto.HeightCM;
            person.WeightKG = dto.WeightKG;
            person.BloodType = dto.BloodType;
            person.GsisID = dto.GsisID;
            person.PagibigID = dto.PagibigID;
            person.PhilhealthID = dto.PhilhealthID;
            person.SssNo = dto.SssNo;
            person.Tin = dto.Tin;
            person.AgencyEmployeeNo = dto.AgencyEmployeeNo;
            person.ResidentialAddress = dto.ResidentialAddress;
            person.ResidentialZip = dto.ResidentialZip;
            person.PermanentAddress = dto.PermanentAddress;
            person.PermanentZip = dto.PermanentZip;
            person.TelephoneNo = dto.TelephoneNo;
            person.MobileNo = dto.MobileNo;
            person.Email = dto.Email;

            person.DepartmentID = dto.DepartmentID;
            person.Designation = dto.Designation;

            // Spouse
            person.SpouseSurname = dto.SpouseSurname;
            person.SpouseFirstName = dto.SpouseFirstName;
            person.SpouseMiddleName = dto.SpouseMiddleName;
            person.SpouseNameExtension = dto.SpouseNameExtension;
            person.SpouseOccupation = dto.SpouseOccupation;
            person.SpouseEmployer = dto.SpouseEmployer;
            person.SpouseBusinessAddress = dto.SpouseBusinessAddress;
            person.SpouseTelephone = dto.SpouseTelephone;

            // Father
            person.FatherSurname = dto.FatherSurname;
            person.FatherFirstName = dto.FatherFirstName;
            person.FatherMiddleName = dto.FatherMiddleName;
            person.FatherNameExtension = dto.FatherNameExtension;

            // Mother
            person.MotherSurname = dto.MotherSurname;
            person.MotherFirstName = dto.MotherFirstName;
            person.MotherMiddleName = dto.MotherMiddleName;

            // Children
            person.Children.Clear();
            foreach (var childDto in dto.Children)
            {
                person.Children.Add(new Child
                {
                    PersonID = id,
                    FullName = childDto.FullName,
                    DateOfBirth = childDto.DateOfBirth
                });
            }

            // Education
            person.EducationRecords.Clear();
            foreach (var eduDto in dto.EducationRecords)
            {
                person.EducationRecords.Add(new Education
                {
                    PersonID = id,
                    Level = eduDto.Level,
                    SchoolName = eduDto.SchoolName,
                    Degree = eduDto.Degree,
                    AttendanceFrom = eduDto.AttendanceFrom,
                    AttendanceTo = eduDto.AttendanceTo,
                    HighestLevel = eduDto.HighestLevel,
                    YearGraduated = eduDto.YearGraduated,
                    Honors = eduDto.Honors
                });
            }

            // Voluntary Work
            person.VoluntaryWorks.Clear();
            foreach (var vwDto in dto.VoluntaryWorks)
            {
                person.VoluntaryWorks.Add(new VoluntaryWork
                {
                    PersonID = id,
                    Organization = vwDto.Organization,
                    DateFrom = vwDto.DateFrom,
                    DateTo = vwDto.DateTo,
                    NumberOfHours = vwDto.NumberOfHours,
                    Position = vwDto.Position
                });
            }

            // Trainings
            person.Trainings.Clear();
            foreach (var tDto in dto.Trainings)
            {
                person.Trainings.Add(new Training
                {
                    PersonID = id,
                    Title = tDto.Title,
                    DateFrom = tDto.DateFrom,
                    DateTo = tDto.DateTo,
                    NumberOfHours = tDto.NumberOfHours,
                    TypeOfLD = tDto.TypeOfLD,
                    ConductedBy = tDto.ConductedBy
                });
            }

            // Skills & Hobbies
            person.SkillsHobbies.Clear();
            foreach (var sDto in dto.SkillsHobbies)
            {
                person.SkillsHobbies.Add(new SkillHobby
                {
                    PersonID = id,
                    SkillOrHobby = sDto.SkillOrHobby
                });
            }

            // Distinctions
            person.Distinctions.Clear();
            foreach (var dDto in dto.Distinctions)
            {
                person.Distinctions.Add(new Distinction
                {
                    PersonID = id,
                    DistinctionName = dDto.Distinction
                });
            }

            // Memberships
            person.Memberships.Clear();
            foreach (var mDto in dto.Memberships)
            {
                person.Memberships.Add(new Membership
                {
                    PersonID = id,
                    OrganizationName = mDto.OrganizationName
                });
            }

            person.CivilServiceEligibilities.Clear();
            foreach (var eDto in dto.CivilServiceEligibilities)
            {
                person.CivilServiceEligibilities.Add(new CivilServiceEligibility
                {
                    PersonID = id,
                    CareerService = eDto.CareerService,
                    Rating = eDto.Rating,
                    DateOfExamination = eDto.DateOfExamination,
                    PlaceOfExamination = eDto.PlaceOfExamination,
                    LicenseNumber = eDto.LicenseNumber,
                    LicenseValidity = eDto.LicenseValidity,
                    DateReleased = eDto.DateReleased
                });
            }

            person.WorkExperiences.Clear();
            foreach (var wDto in dto.WorkExperiences)
            {
                person.WorkExperiences.Add(new WorkExperience
                {
                    PersonID = id,
                    DateFrom = wDto.DateFrom,
                    DateTo = wDto.DateTo,
                    PositionTitle = wDto.PositionTitle,
                    DepartmentAgencyCompany = wDto.DepartmentAgencyCompany,
                    MonthlySalary = wDto.MonthlySalary,
                    SalaryGradeStep = wDto.SalaryGradeStep,
                    StatusOfAppointment = wDto.StatusOfAppointment,
                    IsGovernmentService = wDto.IsGovernmentService,
                    Description = wDto.Description
                });
            }

            // Fetch Department Name based on DepartmentID
            var department = await _db.Department
                .Where(d => d.DepartmentID == dto.DepartmentID)
                .Select(d => d.DepartmentName)
                .FirstOrDefaultAsync();

            var idApplication = await _db.EmployeeIDApplications
                .FirstOrDefaultAsync(p => p.PersonID == id);

            if (idApplication == null)
            {
                idApplication = new EmployeeIDApplication();
                _db.EmployeeIDApplications.Add(idApplication);
                idApplication.Status = 0;
            }
            if (idApplication.DateSchedule  == null)
            {
                idApplication.PersonID = id;
                idApplication.Surname = dto.Surname;
                idApplication.FirstName = dto.FirstName;
                idApplication.MiddleName = dto.MiddleName;
                idApplication.DateOfBirth = dto.DateOfBirth;
                idApplication.EmployeeID = dto.AgencyEmployeeNo;
                idApplication.DepartmentID = dto.DepartmentID;
                idApplication.DepartmentName = department;
                idApplication.Designation = dto.Designation;
            }
            //idApplication.PrintDesignation = false;

            await _db.SaveChangesAsync();
            return Ok(person.PersonID);
        }
    }
}
