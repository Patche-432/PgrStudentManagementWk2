namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;

// Result of checking whether a student's current supervisory team is a completed arrangement.
// A team may be incomplete while assignments are being added one at a time.
public record SupervisoryTeamAssessment(
    bool IsComplete,
    IReadOnlyList<string> Issues);

// One assignment plus the supervisor's name, so views do not need to look supervisors up themselves.
public record SupervisoryTeamEntry(
    SupervisoryAssignment Assignment,
    string SupervisorName);
