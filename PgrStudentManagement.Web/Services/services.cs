using PgrStudentManagement.Web.Data;
using PgrStudentManagement.Web.Models;

namespace PgrStudentManagement.Web.Services;

public class StudentService
{
    private readonly StudentStorage _studentStorage;

    public StudentService(StudentStorage studentStorage)
    {
    _studentStorage = studentStorage;
    }

    public IReadOnlyList<Student> GetStudents()
    {
        return _studentStorage.GetStudents();
    }

    public Student? GetStudent(string studentNumber)
    {
        return _studentStorage.GetStudent(studentNumber);
    }

    public void UpdateStatus(string studentNumber, Status status)
    {
        var student = _studentStorage.GetStudent(studentNumber);
        if (student is not null)
        {
            student.Status = status;
        } 
    }





























    public Student? CreateStudent(string studentNumber, string firstName, string lastName, Status status = Status.Researching,
    string? course = null, 
    string? modeOfStudy = null,
    DateTime? startDate = null,
    string? thesisTitle = null)
    {
        if (string.IsNullOrWhiteSpace(studentNumber) ||
            _studentStorage.GetStudent(studentNumber) is not null)
        {
            return null;
        }

        var student = new Student
        {
            StudentNumber = studentNumber,
            FirstName = firstName,
            LastName = lastName,
            Status = status,
            Course = course,
            ModeOfStudy = modeOfStudy,
            StartDate = startDate,
            ThesisTitle = thesisTitle
        };

        _studentStorage.CreateStudent(student);

        return student;
    }
























    public void UpdateExpectedSubmissionDate(string studentNumber,DateTime? expectedSubmissionDate)
    {
        var student = _studentStorage.GetStudent(studentNumber);
        if (student is not null)
        {
            student.ExpectedSubmissionDate = expectedSubmissionDate;
        }
    }

    public void RecordSubmission(string studentNumber,DateTime? actualSubmissionDate)
    {
        var student = _studentStorage.GetStudent(studentNumber);
        if (student is not null)
        {
            student.ActualSubmissionDate = actualSubmissionDate;
        }
    }
}