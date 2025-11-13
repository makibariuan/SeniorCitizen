using OnlineRegistration.Server.Models;

namespace OnlineRegistration.Server.DTOs
{


    public class ChildDto
    {
        public int ChildID { get; set; }
        public int PersonID { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class EducationDto
    {
        public string SchoolName { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;

        // Use int? if only storing year, DateTime? if full date
        public int? AttendanceFrom { get; set; } = null;
        public int? AttendanceTo { get; set; } = null;

        public string HighestLevel { get; set; } = string.Empty;
        public int? YearGraduated { get; set; } = null;
        public string Honors { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
    }

    public class CivilServiceEligibilityDTO
    {
        public int EligibilityID { get; set; }
        public int PersonID { get; set; }
        public string? CareerService { get; set; }
        public string? Rating { get; set; }
        public DateTime? DateOfExamination { get; set; }
        public string? PlaceOfExamination { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseValidity { get; set; }
        public DateTime? DateReleased { get; set; }
    }

    public class WorkExperienceDTO
    {
        public int WorkExperienceID { get; set; }
        public int PersonID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string PositionTitle { get; set; } = string.Empty;
        public string DepartmentAgencyCompany { get; set; } = string.Empty;
        public decimal? MonthlySalary { get; set; }
        public string? SalaryGradeStep { get; set; }
        public string StatusOfAppointment { get; set; } = string.Empty;
        public bool IsGovernmentService { get; set; }
        public string? Description { get; set; }
    }

    // Voluntary Work
    public class VoluntaryWorkDTO
    {
        public int WorkId { get; set; }
        public int PersonID { get; set; }
        public string Organization { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? NumberOfHours { get; set; }
        public string? Position { get; set; }
    }

    // Training / Learning & Development
    public class TrainingDTO
    {
        public int TrainingId { get; set; }
        public int PersonID { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public decimal? NumberOfHours { get; set; }
        public string? TypeOfLD { get; set; } // Managerial, Supervisory, Technical
        public string? ConductedBy { get; set; }
    }

    // Skills & Hobbies
    public class SkillHobbyDTO
    {
        public int SkillId { get; set; }
        public int PersonID { get; set; }
        public string SkillOrHobby { get; set; } = string.Empty;
    }

    // Distinctions
    public class DistinctionDTO
    {
        public int DistinctionId { get; set; }
        public int PersonID { get; set; }
        public string Distinction { get; set; } = string.Empty;
    }

    // Memberships
    public class MembershipDTO
    {
        public int MembershipId { get; set; }
        public int PersonID { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
    }

    public class PersonalInformationDto
    {
        public int PersonID { get; set; }
        public string? CsIdNo { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? NameExtension { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public int? SexID { get; set; }
        public int? CivilStatusID { get; set; }
        public string? Citizenship { get; set; }
        public decimal? HeightCM { get; set; }
        public decimal? WeightKG { get; set; }
        public string BloodType { get; set; }
        public string GsisID { get; set; }
        public string PagibigID { get; set; }
        public string PhilhealthID { get; set; }
        public string SssNo { get; set; }
        public string Tin { get; set; }
        public string AgencyEmployeeNo { get; set; }
        public string ResidentialAddress { get; set; }
        public string ResidentialZip { get; set; }
        public string PermanentAddress { get; set; }
        public string PermanentZip { get; set; }
        public string TelephoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }

        public int? DepartmentID { get; set; }
        public string? Designation { get; set; }

        // 🔹 Spouse (optional)
        public string? SpouseSurname { get; set; }
        public string? SpouseFirstName { get; set; }
        public string? SpouseMiddleName { get; set; }
        public string? SpouseNameExtension { get; set; }
        public string? SpouseOccupation { get; set; }
        public string? SpouseEmployer { get; set; }
        public string? SpouseBusinessAddress { get; set; }
        public string? SpouseTelephone { get; set; }

        // 🔹 Father (optional)
        public string? FatherSurname { get; set; }
        public string? FatherFirstName { get; set; }
        public string? FatherMiddleName { get; set; }
        public string? FatherNameExtension { get; set; }

        // 🔹 Mother (optional)
        public string? MotherSurname { get; set; }
        public string? MotherFirstName { get; set; }
        public string? MotherMiddleName { get; set; }

        public List<ChildDto> Children { get; set; } = new();
        public List<EducationDto> EducationRecords { get; set; } = new();
        public List<VoluntaryWorkDTO> VoluntaryWorks { get; set; } = new();
        public List<TrainingDTO> Trainings { get; set; } = new();
        public List<SkillHobbyDTO> SkillsHobbies { get; set; } = new();
        public List<DistinctionDTO> Distinctions { get; set; } = new();
        public List<MembershipDTO> Memberships { get; set; } = new();
        public List<CivilServiceEligibilityDTO> CivilServiceEligibilities { get; set; } = new();
        public List<WorkExperienceDTO> WorkExperiences { get; set; } = new();
    }


}
