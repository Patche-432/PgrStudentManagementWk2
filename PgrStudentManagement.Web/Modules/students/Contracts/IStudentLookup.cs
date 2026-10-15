namespace PgrStudentManagement.Web.Modules.Students.Contracts;

public interface IStudentLookup
{
    // Returns null if no student has the given student number.
    StudentSummary? GetStudentSummary(string studentNumber);
}
