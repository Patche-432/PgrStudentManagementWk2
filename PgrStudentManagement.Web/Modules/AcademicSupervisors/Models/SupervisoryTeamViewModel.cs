namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;

// View model for W4-UC05: a student's current and historical supervisory assignments.
public class SupervisoryTeamViewModel
{
    public string StudentNumber { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public IReadOnlyList<SupervisoryTeamEntry> Entries { get; set; } = Array.Empty<SupervisoryTeamEntry>();

    public SupervisoryTeamAssessment Assessment { get; set; } =
        new SupervisoryTeamAssessment(false, Array.Empty<string>());
}
