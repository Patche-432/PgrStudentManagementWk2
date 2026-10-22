using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Web.Modules.Students.Models;
namespace PgrStudentManagement.Web.Modules.Students.Data;
public static class StudentDataSeeder
{
    public static async Task SeedAsync(StudentManagementDbContext context)
    {
        if (await context.Students.AnyAsync())
        {
            return;
        }
        var studentOne = new Student
        {
            StudentNumber = "S100001",
            FirstName = "Alex",
            LastName = "Morgan",
            EmailAddress = "alex.morgan@example.test"
        };
        var studentTwo = new Student
        {
            StudentNumber = "S100002",
            FirstName = "Sam",
            LastName = "Patel",
            EmailAddress = "sam.patel@example.test"
        };
        var studentThree = new Student
        {
            StudentNumber = "S100003",
            FirstName = "Jordan",
            LastName = "Taylor",
            EmailAddress = "jordan.taylor@example.test"
        };
        var studentFour = new Student
        {
            StudentNumber = "S100004",
            FirstName = "Casey",
            LastName = "Wilson",
            EmailAddress = "casey.wilson@example.test"
        };
        var enrolmentOne = new Enrolment
        {
            EnrolmentId = 1,
            StudentNumber = studentOne.StudentNumber,
            ProgrammeCode = "PHD-COMP",
            StartDate = new DateTime(2024, 10, 1),
            ExpectedEndDate = new DateTime(2027, 9, 30),
            ModeOfStudy = "Full-time",
            Status = EnrolmentStatus.Active
        };
        var enrolmentTwo = new Enrolment
        {
            EnrolmentId = 2,
            StudentNumber = studentTwo.StudentNumber,
            ProgrammeCode = "MPHIL-COMP",
            StartDate = new DateTime(2022, 10, 1),
            ExpectedEndDate = new DateTime(2024, 9, 30),
            ModeOfStudy = "Full-time",
            Status = EnrolmentStatus.Completed
        };
        var enrolmentThree = new Enrolment
        {
            EnrolmentId = 3,
            StudentNumber = studentTwo.StudentNumber,
            ProgrammeCode = "PHD-COMP",
            StartDate = new DateTime(2024, 10, 1),
            ExpectedEndDate = new DateTime(2030, 9, 30),
            ModeOfStudy = "Part-time",
            Status = EnrolmentStatus.Active
        };
        var enrolmentFour = new Enrolment
        {
            EnrolmentId = 4,
            StudentNumber = studentThree.StudentNumber,
            ProgrammeCode = "PHD-DESIGN",
            StartDate = new DateTime(2025, 10, 1),
            ExpectedEndDate = null,
            ModeOfStudy = "Full-time",
            Status = EnrolmentStatus.Suspended
        };
        context.Students.AddRange(
        studentOne,
        studentTwo,
        studentThree,
        studentFour);
        context.Enrolments.AddRange(
        enrolmentOne,
        enrolmentTwo,
        enrolmentThree,
        enrolmentFour);
        await context.SaveChangesAsync();
    }
}