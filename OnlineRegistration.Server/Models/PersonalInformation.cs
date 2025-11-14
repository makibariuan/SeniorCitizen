using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineRegistration.Server.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PersonalInformation
    {
        public int PersonID { get; set; }

        public string? CsIdNo { get; set; }
        public string? Surname { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? NameExtension { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public int? SexID { get; set; }
        public int? CivilStatusID { get; set; }
        public string? Citizenship { get; set; }
        public decimal? HeightCM { get; set; }
        public decimal? WeightKG { get; set; }
        public string? BloodType { get; set; }
        public string? GsisID { get; set; }
        public string? PagibigID { get; set; }
        public string? PhilhealthID { get; set; }
        public string? SssNo { get; set; }
        public string? Tin { get; set; }
        public string? AgencyEmployeeNo { get; set; }
        public string? ResidentialAddress { get; set; }
        public string? ResidentialZip { get; set; }
        public string? PermanentAddress { get; set; }
        public string? PermanentZip { get; set; }
        public string? TelephoneNo { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public int? DepartmentID { get; set; }
        public string? Designation { get; set; } = string.Empty;

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

        // 🔹 Navigation properties (Children / Education)
        public ICollection<Child> Children { get; set; } = new List<Child>();
        public ICollection<Education> EducationRecords { get; set; } = new List<Education>();
        public ICollection<CivilServiceEligibility> CivilServiceEligibilities { get; set; } = new List<CivilServiceEligibility>();
        public ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
        public ICollection<VoluntaryWork> VoluntaryWorks { get; set; } = new List<VoluntaryWork>();
        public ICollection<Training> Trainings { get; set; } = new List<Training>();
        public ICollection<SkillHobby> SkillsHobbies { get; set; } = new List<SkillHobby>();
        public ICollection<Distinction> Distinctions { get; set; } = new List<Distinction>();
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }

    [Table("PersonalInformation_CivilServiceEligibility")]
    public class CivilServiceEligibility
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

        // Navigation
        [JsonIgnore] // 👈 prevents cycles
        public PersonalInformation Person { get; set; }
    }

    [Table("PersonalInformation_WorkExperience")]
    public class WorkExperience
    {
        [Key]
        public int WorkExperienceId { get; set; }

        [ForeignKey("PersonalInformation")]
        public int PersonID { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        [Required, MaxLength(150)]
        public string PositionTitle { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string DepartmentAgencyCompany { get; set; } = string.Empty;

        public decimal? MonthlySalary { get; set; }

        [MaxLength(20)]
        public string? SalaryGradeStep { get; set; }

        [Required, MaxLength(100)]
        public string StatusOfAppointment { get; set; } = string.Empty;

        public bool IsGovernmentService { get; set; }

        public string? Description { get; set; }

        // Navigation property
        [JsonIgnore] // prevents reference loops
        public PersonalInformation Person { get; set; }
    }


    [Table("PersonalInformation_Children")]
    public class Child
    {
        public int ChildId { get; set; }
        public int PersonID { get; set; }

        [Column("Child_Name")]
        public string FullName { get; set; } = string.Empty;

        [Column("Child_DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        // Navigation
        [JsonIgnore] // 👈 prevents cycles
        public PersonalInformation? Person { get; set; }
    }

    [Table("PersonalInformation_Education")]
    public class Education
    {
        public int EducationId { get; set; }
        public int PersonID { get; set; }
        public string Level { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;

        [Column("DegreeCourse")]
        public string? Degree { get; set; }

        public int? AttendanceFrom { get; set; }
        public int? AttendanceTo { get; set; }
        public string? HighestLevel { get; set; }
        public int? YearGraduated { get; set; }
        public string? Honors { get; set; }

        // Navigation
        [JsonIgnore] // 👈 prevents cycles
        public PersonalInformation? Person { get; set; }
    }

    [Table("PersonalInformation_VoluntaryWork")]
    public class VoluntaryWork
    {
        public int WorkId { get; set; }
        public int PersonID { get; set; }
        public string Organization { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? NumberOfHours { get; set; }
        public string? Position { get; set; }

        // Navigation
        [JsonIgnore]
        public PersonalInformation? Person { get; set; }
    }

    [Table("PersonalInformation_Training")]
    public class Training
    {
        public int TrainingId { get; set; }
        public int PersonID { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public decimal? NumberOfHours { get; set; }
        public string? TypeOfLD { get; set; }
        public string? ConductedBy { get; set; }

        // Navigation
        [JsonIgnore]
        public PersonalInformation? Person { get; set; }
    }

    [Table("PersonalInformation_SkillsHobbies")]
    public class SkillHobby
    {
        public int SkillId { get; set; }
        public int PersonID { get; set; }
        public string SkillOrHobby { get; set; } = string.Empty;

        [JsonIgnore]
        public PersonalInformation? Person { get; set; }
    }

    [Table("PersonalInformation_Distinctions")]
    public class Distinction
    {
        public int DistinctionId { get; set; }
        public int PersonID { get; set; }
        public string DistinctionName { get; set; } = string.Empty;

        [JsonIgnore]
        public PersonalInformation? Person { get; set; }
    }

    [Table("PersonalInformation_Memberships")]
    public class Membership
    {
        public int MembershipId { get; set; }
        public int PersonID { get; set; }
        public string OrganizationName { get; set; } = string.Empty;

        [JsonIgnore]
        public PersonalInformation? Person { get; set; }
    }

    [Table("EmployeeIDApplication")]
    public class EmployeeIDApplication
    {
        [Key]
        public int PersonID { get; set; }
        public string? Surname { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set;}
        public DateTime? DateOfBirth { get; set; }
        public string? EmployeeID { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public string? Designation { get; set; }
        public bool? PrintDesignation { get; set; }
        public string? ApplicationCode {  get; set; }
        public DateTime? DateSchedule {  get; set; }
        public DateTime? DateCapture { get; set; }
        public DateTime? DateUpload { get; set; }
        //public DateTime? DateActivate { get; set; }
        public string? Photo { get; set; }
        public string? Signature { get; set; }
        public string? LeftThumb { get; set; }
        public string? LeftIndex { get; set; }
        public string? LeftMiddle { get; set; }
        public string? LeftRing { get; set; }
        public string? LeftSmall { get; set; }
        public string? RightThumb { get; set; }
        public string? RightIndex { get; set; }
        public string? RightMiddle { get; set; }
        public string? RightRing { get; set; }
        public string? RightSmall { get; set; }
        public string? EyeLeft { get; set; }
        public string? EyeRight { get; set; }
        public string? BiometricLeft { get; set; }
        public string? BiometricRight { get; set; }
        public int? Status { get; set; } 

        public int? AFISHit { get; set; }
        public DateTime? AFISDateProcess {  get; set; }
        public int? AFISPersonHit { get; set; }
    }
}
