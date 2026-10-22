using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Web.Modules.Students.Models;

namespace PgrStudentManagement.Web.Modules.Students.Data;

public class StudentManagementDbContext : DbContext
{
    public StudentManagementDbContext(DbContextOptions<StudentManagementDbContext> options): base(options)
    {
    }
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Enrolment>()
        .HasOne(enrolment => enrolment.Student)
        .WithMany(student => student.Enrolments)
        .HasForeignKey(enrolment =>
        enrolment.StudentNumber)
        .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>()
        .Property(enrolment => enrolment.Status)
        .HasConversion<string>()
        .HasMaxLength(30);
    }
}