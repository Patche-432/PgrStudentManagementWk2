using PgrStudentManagement.Web.Models;
using PgrStudentManagement.Web.Services;
namespace PgrStudentManagement.Web.Data;

public class StudentStorage
{
    private static List<Student> _students = new()
    {
        new Student
        {
            StudentNumber = "S100001",
            FirstName = "John",
            LastName = "Doe",
            Course = "PhD Computing",
            ModeOfStudy = "Full-time",
            StartDate = new DateTime(2025, 10, 1),
            Status = Status.Researching,
            ThesisTitle = "Explainable Models for Clinical Decision Support"
        },
        new Student
        {
            StudentNumber = "S100002",
            FirstName = "Jane",
            LastName = "Smith",
            Course = "Professional Doctorate",
            ModeOfStudy = "Part-time",
            StartDate = new DateTime(2024, 10, 1),
            Status = Status.WritingUp,
            ThesisTitle = null
        }
    };
    public static void ResetDefaultStudents()
    {
        _students.Clear();
        _students.AddRange(new List<Student>
        {
            new Student
            {
                StudentNumber = "S100001",
                FirstName = "John",
                LastName = "Doe",
                Course = "PhD Computing",
                ModeOfStudy = "Full-time",
                StartDate = new DateTime(2025, 10, 1),
                Status = Status.Researching,
                ThesisTitle = "Explainable Models for Clinical Decision Support",
                ExpectedSubmissionDate = new DateTime(2028, 9, 30),
                OriginalExpectedSubmissionDate = new DateTime(2028, 9, 30),
                ActualSubmissionDate = null
            },
            new Student
            {
                StudentNumber = "S100002",
                FirstName = "Jane",
                LastName = "Smith",
                Course = "Professional Doctorate",
                ModeOfStudy = "Part-time",
                StartDate = new DateTime(2024, 10, 1),
                Status = Status.WritingUp,
                ThesisTitle = null,
                ExpectedSubmissionDate = null,
                OriginalExpectedSubmissionDate = null,
                ActualSubmissionDate = null
            }
        });
    }













    public void CreateStudent(Student student)
    {
       if (student is null)
        {
            throw new ArgumentNullException(nameof(student));
        }
        var existingIndex = _students.FindIndex(s => s.StudentNumber == student.StudentNumber);
        if (existingIndex >= 0)
        {
            throw new InvalidOperationException($"Student with number '{student.StudentNumber}' already exists.");
        }
        _students.Add(student);
    }














    public IReadOnlyList<Student> GetStudents()
    {
        return _students.ToList();
    }
    
    public Student? GetStudent(string studentNumber)
    {
        return _students.FirstOrDefault(s => s.StudentNumber == studentNumber);
    }

    public void AddStudent(Student student)
    {
        _students.Add(student);
    }

    public void UpdateStudent(Student student)
    {
        var index = _students.FindIndex(s => s.StudentNumber == student.StudentNumber);
        if (index < 0)
            throw new KeyNotFoundException($"Student {student.StudentNumber} not found.");
            _students[index] = student;
            
    }    
}
