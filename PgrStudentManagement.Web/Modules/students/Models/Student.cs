namespace PgrStudentManagement.Web.Modules.Students.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

[Index(nameof(EmailAddress), IsUnique = true)]
public class Student
{
    [Key]
    [MaxLength(20)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string EmailAddress { get; set; } = string.Empty;

    public ICollection<Enrolment> Enrolments { get; set; }
        = new List<Enrolment>();

    // Legacy in-memory properties
    [NotMapped] public Status Status { get; set; } = Status.Researching;
    [NotMapped] public string FullName => $"{FirstName} {LastName}";
    [NotMapped] public string? Course { get; set; }
    [NotMapped] public string? ModeOfStudy { get; set; }
    [NotMapped] public DateTime? StartDate { get; set; }
    [NotMapped] public string? ThesisTitle { get; set; }
    [NotMapped] public DateTime? ExpectedSubmissionDate { get; set; }
    [NotMapped] public DateTime? OriginalExpectedSubmissionDate { get; set; }
    [NotMapped] public DateTime? ActualSubmissionDate { get; set; }
}

