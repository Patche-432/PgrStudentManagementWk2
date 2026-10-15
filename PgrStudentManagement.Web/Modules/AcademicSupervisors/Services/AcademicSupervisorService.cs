using PgrStudentManagement.Web.Modules.AcademicSupervisors.Data;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;
using PgrStudentManagement.Web.Modules.Students.Contracts;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Services;

public class AcademicSupervisorService
{
    private readonly AcademicSupervisorStorage _storage;
    private readonly IStudentLookup _studentLookup;

    public AcademicSupervisorService(AcademicSupervisorStorage storage, IStudentLookup studentLookup)
    {
        _storage = storage;
        _studentLookup = studentLookup;
    }

    // Asks the Students module, through its public contract only, for limited student information.
    // Used by the assignment use cases (W4-UC04 onwards).
    public StudentSummary? GetStudentSummary(string studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            return null;
        }

        return _studentLookup.GetStudentSummary(studentNumber.Trim());
    }

    // W4-UC02: the ordering rule is applied here, not in the storage class.
    // Ordered by last name, then first name, then staff number.
    public IReadOnlyList<AcademicSupervisor> GetAcademicSupervisors()
    {
        return _storage.GetAcademicSupervisors()
            .OrderBy(s => s.LastName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(s => s.FirstName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(s => s.StaffNumber, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    // W4-UC03
    public AcademicSupervisor? GetAcademicSupervisor(string staffNumber)
    {
        if (string.IsNullOrWhiteSpace(staffNumber))
        {
            return null;
        }

        return _storage.GetAcademicSupervisor(staffNumber.Trim());
    }

    // W4-UC01 business rule: every supervisor has a unique staff number.
    public bool StaffNumberExists(string staffNumber)
    {
        return GetAcademicSupervisor(staffNumber) is not null;
    }

    // W4-UC01: returns null if the staff number is missing or already in use.
    // Eligible roles are derived from employment type and training, so they are not set here.
    // The controller checks mandatory fields (ModelState) before calling this.
    public AcademicSupervisor? CreateAcademicSupervisor(AcademicSupervisor input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var staffNumber = input.StaffNumber?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(staffNumber) || StaffNumberExists(staffNumber))
        {
            return null;
        }

        var supervisor = new AcademicSupervisor
        {
            StaffNumber = staffNumber,
            FirstName = input.FirstName.Trim(),
            LastName = input.LastName.Trim(),
            EmailAddress = string.IsNullOrWhiteSpace(input.EmailAddress) ? null : input.EmailAddress.Trim(),
            EmploymentType = input.EmploymentType,
            TrainingCompleted = input.TrainingCompleted,
        };

        _storage.AddAcademicSupervisor(supervisor);

        return supervisor;
    }

    // W4-UC04: creates one assignment if the immediate rules allow it.
    // Team-completeness rules are NOT applied here; see EvaluateSupervisoryTeam.
    public AssignmentOutcome AssignSupervisor(string studentNumber, string staffNumber, SupervisoryRole role, DateTime startDate, bool exceptionAuthorised)
    {
        var student = GetStudentSummary(studentNumber);
        if (student is null)
        {
            return AssignmentOutcome.StudentNotFound;
        }

        var supervisor = GetAcademicSupervisor(staffNumber);
        if (supervisor is null)
        {
            return AssignmentOutcome.SupervisorNotFound;
        }

        var current = _storage.GetAssignmentsForStudent(student.StudentNumber)
            .Where(a => a.IsCurrent)
            .ToList();

        // Assignment Already Exists alternative flow (current assignments only).
        if (current.Any(a => a.Role == role && string.Equals(a.StaffNumber, supervisor.StaffNumber, StringComparison.OrdinalIgnoreCase)))
        {
            return AssignmentOutcome.DuplicateAssignment;
        }

        // BR01 / BR13: external supervisors cannot hold the two required roles.
        if (supervisor.EmploymentType == SupervisorEmploymentType.External &&
            role is SupervisoryRole.DirectorOfStudies or SupervisoryRole.FirstSupervisor)
        {
            return AssignmentOutcome.EmploymentTypeNotPermitted;
        }

        // Eligibility is derived from employment type and training.
        if (!supervisor.IsEligibleFor(role))
        {
            return AssignmentOutcome.SupervisorNotEligible;
        }

        // BR14: unique roles only (not SecondSupervisor or Adviser).
        if (role is SupervisoryRole.DirectorOfStudies or SupervisoryRole.FirstSupervisor && current.Any(a => a.Role == role))
        {
            return AssignmentOutcome.RoleAlreadyFilled;
        }

        // BR03: a fourth current supervisor needs an authorised exception.
        // Decision: Advisers do not count towards the limit, and one person
        // holding two roles counts once.
        var counted = current
            .Where(a => a.Role != SupervisoryRole.Adviser)
            .Select(a => a.StaffNumber)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var isFourthSupervisor =
            role != SupervisoryRole.Adviser &&
            counted.Count >= 3 &&
            !counted.Contains(supervisor.StaffNumber, StringComparer.OrdinalIgnoreCase);

        if (isFourthSupervisor && !exceptionAuthorised)
        {
            return AssignmentOutcome.ExceptionRequired;
        }

        _storage.AddSupervisoryAssignment(new SupervisoryAssignment
        {
            StudentNumber = student.StudentNumber,
            StaffNumber = supervisor.StaffNumber,
            Role = role,
            StartDate = startDate.Date,                 // decision: date only, no time
            ExceptionAuthorised = isFourthSupervisor    // recorded on the fourth assignment
        });

        return AssignmentOutcome.Created;
    }

    // W4-UC05: all assignments for a student, current first, then by role and start date.
    public IReadOnlyList<SupervisoryTeamEntry> GetSupervisoryTeam(string studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            return Array.Empty<SupervisoryTeamEntry>();
        }

        return _storage.GetAssignmentsForStudent(studentNumber.Trim())
            .OrderByDescending(a => a.IsCurrent)
            .ThenBy(a => a.Role)
            .ThenBy(a => a.StartDate)
            .Select(a => new SupervisoryTeamEntry(
                a,
                _storage.GetAcademicSupervisor(a.StaffNumber)?.FullName ?? a.StaffNumber))
            .ToList();
    }

    // Evaluates the rules for a COMPLETED supervisory arrangement (BR01, BR02, BR03).
    // An incomplete team is allowed while assignments are added; this reports what is missing.
    public SupervisoryTeamAssessment EvaluateSupervisoryTeam(string studentNumber)
    {
        var issues = new List<string>();

        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            issues.Add("No student was specified.");
            return new SupervisoryTeamAssessment(false, issues);
        }

        var current = _storage.GetAssignmentsForStudent(studentNumber.Trim())
            .Where(a => a.IsCurrent)
            .ToList();

        bool HasInstitutionEmployed(SupervisoryRole role) =>
            current.Any(a =>
                a.Role == role &&
                _storage.GetAcademicSupervisor(a.StaffNumber)?.EmploymentType == SupervisorEmploymentType.InstitutionEmployed);

        // BR01
        if (!HasInstitutionEmployed(SupervisoryRole.DirectorOfStudies))
        {
            issues.Add("The student needs an institution-employed Director of Studies.");
        }

        if (!HasInstitutionEmployed(SupervisoryRole.FirstSupervisor))
        {
            issues.Add("The student needs an institution-employed First Supervisor.");
        }

        // BR02 / BR03. Advisers do not count; one person holding two roles counts once.
        var supervisorCount = current
            .Where(a => a.Role != SupervisoryRole.Adviser)
            .Select(a => a.StaffNumber)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        if (supervisorCount < 2)
        {
            issues.Add($"A completed team normally has two or three supervisors; this student has {supervisorCount}.");
        }

        if (supervisorCount > 3 && !current.Any(a => a.ExceptionAuthorised))
        {
            issues.Add("A fourth supervisor requires an authorised exception.");
        }

        return new SupervisoryTeamAssessment(issues.Count == 0, issues);
    }
}
