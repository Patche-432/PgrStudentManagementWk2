namespace PgrStudentManagement.Web.Modules.Students.Models;

public class Student
{
    public string StudentNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string? Course { get; set; }

    public string? ModeOfStudy { get; set; }

    public DateTime? StartDate { get; set; }

    public Status Status { get; set; }

    public string? ThesisTitle { get; set; }

    public DateTime? ExpectedSubmissionDate { get; set; }

    public DateTime? OriginalExpectedSubmissionDate { get; set; }

    public DateTime? ActualSubmissionDate { get; set; }

    public bool HasThesisInformation =>
        !string.IsNullOrWhiteSpace(ThesisTitle) ||
        ExpectedSubmissionDate.HasValue ||
        ActualSubmissionDate.HasValue;
}