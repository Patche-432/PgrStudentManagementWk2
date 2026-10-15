using PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Data;

public class AcademicSupervisorStorage
{
    private readonly List<AcademicSupervisor> _academicSupervisors = new()
    {
        new AcademicSupervisor
        {
            StaffNumber = "E100001",
            FirstName = "Alice",
            LastName = "Morgan",
            EmailAddress = "alice.morgan@example.ac.uk",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
        },
        new AcademicSupervisor
        {
            StaffNumber = "X200001",
            FirstName = "Raj",
            LastName = "Patel",
            EmailAddress = null,
            EmploymentType = SupervisorEmploymentType.External,
            TrainingCompleted = false,
        }
    };

    private readonly List<SupervisoryAssignment> _supervisoryAssignments = new();

    public IReadOnlyList<AcademicSupervisor> GetAcademicSupervisors()
    {
        return _academicSupervisors.ToList();
    }

    public AcademicSupervisor? GetAcademicSupervisor(string staffNumber)
    {
        return _academicSupervisors.FirstOrDefault(s =>
            string.Equals(s.StaffNumber, staffNumber, StringComparison.OrdinalIgnoreCase));
    }

    public void AddAcademicSupervisor(AcademicSupervisor supervisor)
    {
        ArgumentNullException.ThrowIfNull(supervisor);
        _academicSupervisors.Add(supervisor);
    }

    public IReadOnlyList<SupervisoryAssignment> GetAssignmentsForStudent(string studentNumber)
    {
        return _supervisoryAssignments
            .Where(a => string.Equals(a.StudentNumber, studentNumber, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public IReadOnlyList<SupervisoryAssignment> GetAssignmentsForSupervisor(string staffNumber)
    {
        return _supervisoryAssignments
            .Where(a => string.Equals(a.StaffNumber, staffNumber, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void AddSupervisoryAssignment(SupervisoryAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);
        _supervisoryAssignments.Add(assignment);
    }

    public void UpdateSupervisoryAssignment(SupervisoryAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        var index = _supervisoryAssignments.FindIndex(a => ReferenceEquals(a, assignment));
        if (index < 0)
        {
            throw new KeyNotFoundException("Supervisory assignment not found.");
        }

        _supervisoryAssignments[index] = assignment;
    }
}
