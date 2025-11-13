using System.ComponentModel.DataAnnotations;

namespace OnlineRegistration.Server.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}
