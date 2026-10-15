namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;

// Links one student (owned by the Students module) to one Academic Supervisor (owned by this module).
public class SupervisoryAssignment
{
    public string StudentNumber { get; set; } = string.Empty;

    public string StaffNumber { get; set; } = string.Empty;

    public SupervisoryRole Role { get; set; }

    public DateTime StartDate { get; set; }

    // Absent while the assignment is current.
    public DateTime? EndDate { get; set; }

    // Derived from endDate, as in the domain model; not stored separately.
    public bool IsCurrent => !EndDate.HasValue;

    // Records that a fourth supervisor (BR03) has been authorised.
    public bool ExceptionAuthorised { get; set; }
}
