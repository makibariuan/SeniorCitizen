using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineRegistration.Server.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Employee
    {
        public int PersonID { get; set; }
        public string? Surname { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public int? Sex { get; set; }
        public string? CivilStatus { get; set; }
        public string? Citizenship { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? BloodType { get; set; }
        public string? Nationality { get; set; }
        public string? Department { get; set; }
        public string? EmployeeID { get; set; }
        public string? ResidentialAddress { get; set; }
        public string? PermanentAddress { get; set; }

        //public string? FatherSurname { get; set; }
        //public string?  FatherFirstName { get; set; }
        //public string? FatherMiddleName { get; set; }
        //public string? MotherSurname { get; set; }
        //public string? MotherFirstName { get; set; }
        //public string? MotherMiddleName { get; set; }
        public DateTime? Schedule { get; set; }
        public int Status { get; set; } = 0;
    }

    public class EmployeeAttendance
    {
        public string EmployeeID { get; set; }
        public string BiometricDeviceID { get; set; }
        public int Mode { get; set; }
        public string? Data { get; set; }
        public DateTime DateLog { get; set; }
        public string? EventType { get; set; }

        public BiometricDevice BiometricDevice { get; set; }
    }

}
