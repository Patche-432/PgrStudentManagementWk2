using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Models;
using PgrStudentManagement.Web.Services;

namespace PgrStudentManagement.Web.Controllers;

public class StudentsController : Controller
{
    private readonly StudentService _studentService;

    public StudentsController(StudentService studentService)
    {
        _studentService = studentService;
    }
    
    public IActionResult Index()
    {
        var students = _studentService.GetStudents();
        return View(students);
    }

    // W2-UC01: View Enrolment Details
    public IActionResult EnrolmentDetails(string? studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            ViewBag.StudentNumber = studentNumber ?? string.Empty;
            return View("StudentNotFound");
        }

        var student = _studentService.GetStudent(studentNumber);

        if (student == null)
        {
            ViewBag.StudentNumber = studentNumber;
            return View("StudentNotFound");
        }

        return View(student);
    }

    // W2-UC02: View Thesis Details
    public IActionResult ThesisDetails(string? studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            ViewBag.StudentNumber = studentNumber ?? string.Empty;
            return View("StudentNotFound");
        }

        var student = _studentService.GetStudent(studentNumber);

        if (student == null)
        {
            ViewBag.StudentNumber = studentNumber;
            return View("StudentNotFound");
        }

        return View(student);
    }

    // W2-UC03: Update Student Status (GET)
    [HttpGet]
    public IActionResult EditStatus(string? studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            ViewBag.StudentNumber = studentNumber ?? string.Empty;
            return View("StudentNotFound");
        }

        var student = _studentService.GetStudent(studentNumber);

        if (student == null)
        {
            ViewBag.StudentNumber = studentNumber;
            return View("StudentNotFound");
        }

        return View(student);
    }








    public IActionResult CreateStudent()
    {
        return View();
    }












// W? -UC? create student post
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateStudent(Student student)
    {
        if (!ModelState.IsValid)
        {
            return View(student);
        }
        var createdStudent = _studentService.CreateStudent(
        student.StudentNumber,
        student.FirstName,
        student.LastName,
        student.Status,
        student.Course,
        student.ModeOfStudy,
        student.StartDate,
        student.ThesisTitle);

        if (createdStudent is null)
        {
            ModelState.AddModelError(string.Empty, "Failed to create student. Student number may already exist.");
                return View(student);
        }

        return RedirectToAction(nameof(Index));
    }





















    // W2-UC03: Update Student Status (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditStatus(string? studentNumber, Status status)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            ViewBag.StudentNumber = studentNumber ?? string.Empty;
            return View("StudentNotFound");
        }

        var student = _studentService.GetStudent(studentNumber);

        if (student == null)
        {
            ViewBag.StudentNumber = studentNumber;
            return View("StudentNotFound");
        }

        if (!Enum.IsDefined(typeof(Status), status))
        {
            ModelState.AddModelError(nameof(status), "Invalid status value provided.");
            return View(student);
        }

        student.Status = status;

        TempData["SuccessMessage"] = $"Status updated to {status} for student {student.StudentNumber}.";
        return RedirectToAction(nameof(EnrolmentDetails), new { studentNumber = student.StudentNumber });
    }

    // W2-UC04: Update Expected Thesis Submission Date (GET)
    [HttpGet]
    public IActionResult EditExpectedSubmissionDate(string? studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            ViewBag.StudentNumber = studentNumber ?? string.Empty;
            return View("StudentNotFound");
        }

        var student = _studentService.GetStudent(studentNumber);

        if (student == null)
        {
            ViewBag.StudentNumber = studentNumber;
            return View("StudentNotFound");
        }

        return View(student);
    }

    // W2-UC04: Update Expected Thesis Submission Date (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditExpectedSubmissionDate(string? studentNumber, DateTime? expectedSubmissionDate)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            ViewBag.StudentNumber = studentNumber ?? string.Empty;
            return View("StudentNotFound");
        }

        var student = _studentService.GetStudent(studentNumber);

        if (student == null)
        {
            ViewBag.StudentNumber = studentNumber;
            return View("StudentNotFound");
        }

        if (!expectedSubmissionDate.HasValue)
        {
            ModelState.AddModelError("expectedSubmissionDate", "Invalid expected submission date");
            return View(student);
        }

        if (student.StartDate.HasValue && expectedSubmissionDate.Value < student.StartDate.Value)
        {
            ModelState.AddModelError("expectedSubmissionDate", "Invalid expected submission date: Expected date cannot be prior to start date.");
            return View(student);
        }

        student.ExpectedSubmissionDate = expectedSubmissionDate.Value;

        TempData["SuccessMessage"] = $"Expected submission date updated to {expectedSubmissionDate.Value:yyyy-MM-dd} for student {student.StudentNumber}.";
        return RedirectToAction(nameof(ThesisDetails), new { studentNumber = student.StudentNumber });
    }

    // W2-UC05: Record Actual Thesis Submission (GET)
    [HttpGet]
    public IActionResult RecordSubmission(string? studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            ViewBag.StudentNumber = studentNumber ?? string.Empty;
            return View("StudentNotFound");
        }

        var student = _studentService.GetStudent(studentNumber);

        if (student == null)
        {
            ViewBag.StudentNumber = studentNumber;
            return View("StudentNotFound");
        }

        return View(student);
    }

    // W2-UC05: Record Actual Thesis Submission (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RecordSubmission(string? studentNumber, DateTime? actualSubmissionDate)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            ViewBag.StudentNumber = studentNumber ?? string.Empty;
            return View("StudentNotFound");
        }

        var student = _studentService.GetStudent(studentNumber);

        if (student == null)
        {
            ViewBag.StudentNumber = studentNumber;
            return View("StudentNotFound");
        }

        if (!actualSubmissionDate.HasValue)
        {
            ModelState.AddModelError("actualSubmissionDate", "Invalid submission date");
            return View(student);
        }

        if (student.StartDate.HasValue && actualSubmissionDate.Value < student.StartDate.Value)
        {
            ModelState.AddModelError("actualSubmissionDate", "Invalid submission date: Submission date cannot be before enrolment start date.");
            return View(student);
        }

        // Apply atomic change: both submission date and status change together
        student.ActualSubmissionDate = actualSubmissionDate.Value;
        student.Status = Status.Submitted;

        TempData["SuccessMessage"] = $"Actual thesis submission recorded on {actualSubmissionDate.Value:yyyy-MM-dd} and status updated to Submitted for student {student.StudentNumber}.";
        return RedirectToAction(nameof(ThesisDetails), new { studentNumber = student.StudentNumber });
    }
}
