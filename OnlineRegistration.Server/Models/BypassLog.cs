using OnlineRegistration.Server.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("BypassLogs")]
public class BypassLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } // Id (PK, int, not null) -> Correct

    public string BiometricDeviceID { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string StepName { get; set; } = string.Empty;

    public string ReasonCode { get; set; } = string.Empty;

    public string ReasonDetails { get; set; } = string.Empty;

    public DateTime DateBypassed { get; set; }
}