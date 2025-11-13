namespace OnlineRegistration.Server.DTOs
{
    public class EmployeeIDPrintDto
    {
        public int PersonID { get; set; }
        public string ApplicationCode { get; set; }
        public string Department { get; set; }
        public string FullName { get; set; }
        public string DateCapture { get; set; }
        public string Photo { get; set; }
    }
    public class EmployeeIDApplicationDto
    {
        public int PersonID { get; set; }
        public string? ApplicationCode { get; set; }
        public string? Surname { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? EmployeeID { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public string? Designation { get; set; }
        public bool? PrintDesignation { get; set; } = false;
        public DateTime? DateSchedule { get; set; }
    }

    public class EmployeeIDBiometricsDto
    {
        public int PersonID { get; set; }
        public string? ApplicationCode { get; set; }
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

        public DateTime? DateCapture { get; set; }
    }

    public class EmployeeAttendanceDto
    {
        public string EmployeeID { get; set; }
        public string BiometricDeviceID { get; set; }
        public int Mode { get; set; }
        public string? Data { get; set; }
        public DateTime DateLog { get; set; }
        public string? EventType { get; set; }

    }
}
