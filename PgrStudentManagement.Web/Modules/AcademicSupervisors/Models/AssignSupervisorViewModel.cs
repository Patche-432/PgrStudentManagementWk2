using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;

// Form model for W4-UC04. Holds only the student number, never a Student entity.
public class AssignSupervisorViewModel
{
    public string StudentNumber { get; set; } = string.Empty;

    // Display only; filled from the Students module contract (StudentSummary.DisplayName).
    public string StudentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Select an Academic Supervisor.")]
    [Display(Name = "Academic Supervisor")]
    public string? StaffNumber { get; set; }

    [Required(ErrorMessage = "Select a supervisory role.")]
    [Display(Name = "Supervisory Role")]
    public SupervisoryRole? Role { get; set; }

    [Required(ErrorMessage = "Enter the assignment start date.")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }

    [Display(Name = "Fourth supervisor exception authorised")]
    public bool ExceptionAuthorised { get; set; }

    [ValidateNever]
    public IReadOnlyList<AcademicSupervisor> Supervisors { get; set; } = Array.Empty<AcademicSupervisor>();

    [ValidateNever]
    public IReadOnlyList<SupervisoryTeamEntry> Team { get; set; } = Array.Empty<SupervisoryTeamEntry>();
}
