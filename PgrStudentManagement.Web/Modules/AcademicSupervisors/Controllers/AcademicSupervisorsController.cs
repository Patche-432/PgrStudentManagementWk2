using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Services;
using PgrStudentManagement.Web.Modules.Students.Contracts;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Controllers;

public class AcademicSupervisorsController : Controller
{
    private readonly AcademicSupervisorService _supervisorService;

    public AcademicSupervisorsController(AcademicSupervisorService supervisorService)
    {
        _supervisorService = supervisorService;
    }

    public IActionResult Index()
    {
        var supervisors = _supervisorService.GetAcademicSupervisors();
        return View(supervisors);
    }

    // W4-UC03: View Academic Supervisor Details
    public IActionResult Details(string? staffNumber)
    {
        if (string.IsNullOrWhiteSpace(staffNumber))
        {
            ViewBag.StaffNumber = staffNumber ?? string.Empty;
            return View("AcademicSupervisorNotFound");
        }

        var supervisor = _supervisorService.GetAcademicSupervisor(staffNumber);

        if (supervisor is null)
        {
            ViewBag.StaffNumber = staffNumber;
            return View("AcademicSupervisorNotFound");
        }

        return View(supervisor);
    }

    // W4-UC01:Create Academic Supervisor
    [HttpGet]
    public IActionResult Create()
    {
        return View(new AcademicSupervisor());
    }

    // W4-UC01: Create Academic Supervisor
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AcademicSupervisor supervisor)
    {
        if (!ModelState.IsValid)
        {
            return View(supervisor);
        }

        // Duplicate Staff Number.
        if (_supervisorService.StaffNumberExists(supervisor.StaffNumber))
        {
            ModelState.AddModelError(
                nameof(AcademicSupervisor.StaffNumber),
                "An Academic Supervisor with this staff number already exists");
            return View(supervisor);
        }

        var created = _supervisorService.CreateAcademicSupervisor(supervisor);

        if (created is null)
        {
            ModelState.AddModelError(string.Empty, "The Academic Supervisor could not be created.");
            return View(supervisor);
        }

        TempData["SuccessMessage"] = $"Academic Supervisor {created.FullName} ({created.StaffNumber}) has been created.";
        return RedirectToAction(nameof(Details), new { staffNumber = created.StaffNumber });
    }

    // W4-UC04: Assign Academic Supervisor to Student (display the form).
    [HttpGet]
    public IActionResult Assign(string? studentNumber)
    {
        var student = _supervisorService.GetStudentSummary(studentNumber ?? string.Empty);

        if (student is null)
        {
            ViewBag.StudentNumber = studentNumber ?? string.Empty;
            return View("StudentNotFound");
        }

        var model = new AssignSupervisorViewModel
        {
            StudentNumber = student.StudentNumber,
            StartDate = DateTime.Today
        };

        return View(PopulateAssignModel(model, student));
    }

    // W4-UC04: Assign Academic Supervisor to Student (submit).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Assign(AssignSupervisorViewModel model)
    {
        var student = _supervisorService.GetStudentSummary(model.StudentNumber);

        if (student is null)
        {
            ViewBag.StudentNumber = model.StudentNumber;
            return View("StudentNotFound");
        }

        if (!ModelState.IsValid)
        {
            return View(PopulateAssignModel(model, student));
        }

        var outcome = _supervisorService.AssignSupervisor(
            student.StudentNumber,
            model.StaffNumber!,
            model.Role!.Value,
            model.StartDate!.Value,
            model.ExceptionAuthorised);

        if (outcome == AssignmentOutcome.Created)
        {
            TempData["SuccessMessage"] = $"The Academic Supervisor has been assigned to {student.DisplayName} as {model.Role}.";
            return RedirectToAction(nameof(SupervisoryTeam), new { studentNumber = student.StudentNumber });
        }

        ModelState.AddModelError(string.Empty, DescribeOutcome(outcome));
        return View(PopulateAssignModel(model, student));
    }

    // W4-UC05: View Student Supervisory Team
    public IActionResult SupervisoryTeam(string? studentNumber)
    {
        var student = _supervisorService.GetStudentSummary(studentNumber ?? string.Empty);

        if (student is null)
        {
            ViewBag.StudentNumber = studentNumber ?? string.Empty;
            return View("StudentNotFound");
        }

        var model = new SupervisoryTeamViewModel
        {
            StudentNumber = student.StudentNumber,
            StudentName = student.DisplayName,
            Entries = _supervisorService.GetSupervisoryTeam(student.StudentNumber),
            Assessment = _supervisorService.EvaluateSupervisoryTeam(student.StudentNumber)
        };

        return View(model);
    }

    private AssignSupervisorViewModel PopulateAssignModel(AssignSupervisorViewModel model, StudentSummary student)
    {
        model.StudentNumber = student.StudentNumber;
        model.StudentName = student.DisplayName;
        model.Supervisors = _supervisorService.GetAcademicSupervisors();
        model.Team = _supervisorService.GetSupervisoryTeam(student.StudentNumber)
            .Where(e => e.Assignment.IsCurrent)
            .ToList();
        return model;
    }

    // Presentation only: turns an outcome into a message.
    private static string DescribeOutcome(AssignmentOutcome outcome) => outcome switch
    {
        AssignmentOutcome.StudentNotFound => "Student not found.",
        AssignmentOutcome.SupervisorNotFound => "Academic Supervisor not found.",
        AssignmentOutcome.DuplicateAssignment => "This Academic Supervisor is already assigned to the student in this role",
        AssignmentOutcome.RoleAlreadyFilled => "The student already has a current supervisor in this role.",
        AssignmentOutcome.SupervisorNotEligible => "This Academic Supervisor is not eligible for the selected role.",
        AssignmentOutcome.EmploymentTypeNotPermitted => "An external supervisor cannot be Director of Studies or First Supervisor.",
        AssignmentOutcome.ExceptionRequired => "A fourth supervisor requires an authorised exception.",
        _ => "The Academic Supervisor could not be assigned."
    };
}
