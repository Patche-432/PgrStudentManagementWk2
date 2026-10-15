namespace PgrStudentManagement.Web.Modules.Students.Contracts;

// The minimum student information other modules need to display a supervisory assignment.
public record StudentSummary(
    string StudentNumber,
    string DisplayName);
