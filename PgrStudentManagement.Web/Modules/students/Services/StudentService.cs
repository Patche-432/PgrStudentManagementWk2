using PgrStudentManagement.Web.Modules.Students.Data;
using PgrStudentManagement.Web.Modules.Students.Models;
using PgrStudentManagement.Web.Modules.Students.Contracts;  
using PgrStudentManagement.Web.Modules.Students.Data.Daos;

namespace PgrStudentManagement.Web.Modules.Students.Services;

public class StudentService : IStudentLookup 
{
    private readonly StudentStorage _studentStorage;
    private readonly EnrolmentDao _enrolmentDao;

    public StudentService(StudentStorage studentStorage, EnrolmentDao enrolmentDao)
    {
        _studentStorage = studentStorage;
        _enrolmentDao = enrolmentDao;
    
    }

    public IReadOnlyList<Student> GetStudents()
    {
        return _studentStorage.GetStudents();
    }

    public Task<IReadOnlyList<Enrolment>>
    GetEnrolmentsForStudentAsync(string studentNumber)
    {
        return _enrolmentDao
        .GetEnrolmentsForStudentAsync(studentNumber);
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




    public Student? CreateStudent(string studentNumber, string firstName, string lastName,
    string programmeCode,
    Status status = Status.Researching,
    string? course = null, 
    string? modeOfStudy = null,
    DateTime? startDate = null,
    string? thesisTitle = null,
    string emailAddress = "")
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
            EmailAddress = emailAddress
        };

        var entrolment = new Enrolment
        {
            StudentNumber = studentNumber,
            ProgrammeCode = programmeCode,
            ModeOfStudy = modeOfStudy ?? string .Empty,
            StartDate = DateTime.Now,
            Status = EnrolmentStatus.Active
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

    public StudentSummary? GetStudentSummary(string studentNumber)
    {
        var student = _studentStorage.GetStudent(studentNumber);
        if (student is null)
        {
            return null;
        }
        return new StudentSummary(student.StudentNumber, student.FullName);
}
}