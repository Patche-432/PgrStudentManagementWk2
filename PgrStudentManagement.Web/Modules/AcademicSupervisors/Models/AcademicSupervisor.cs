using System.ComponentModel.DataAnnotations;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;

public class AcademicSupervisor
{
    // Mandatory. Uniquely identifies the supervisor, institution-employed or external.
    [Required(ErrorMessage = "Staff number is required.")]
    [Display(Name = "Staff Number")]
    public string StaffNumber { get; set; } = string.Empty;

    // Mandatory.
    [Required(ErrorMessage = "Last name is required.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    // Mandatory.
    [Required(ErrorMessage = "First name is required.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    // Optional.
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email Address")]
    public string? EmailAddress { get; set; }

    // Mandatory. Nullable so the form forces an explicit choice instead of defaulting.
    [Required(ErrorMessage = "Employment type is required.")]
    [Display(Name = "Employment Type")]
    public SupervisorEmploymentType? EmploymentType { get; set; }

    // Initial training information. Defaults to false (not yet completed).
    [Display(Name = "Training Completed")]
    public bool TrainingCompleted { get; set; }

// Derived from employment type and training; not stored.
    [Display(Name = "Eligible Roles")]
    public IReadOnlyList<SupervisoryRole> EligibleRoles
    {
        get
        {
            var roles = new List<SupervisoryRole>();

            if (EmploymentType == SupervisorEmploymentType.InstitutionEmployed && TrainingCompleted)
            {
                roles.Add(SupervisoryRole.DirectorOfStudies);
                roles.Add(SupervisoryRole.FirstSupervisor);
            }

            if (TrainingCompleted)
            {
                roles.Add(SupervisoryRole.SecondSupervisor);
            }

            roles.Add(SupervisoryRole.Adviser);
            return roles;
        }
    }

    public bool IsEligibleFor(SupervisoryRole role) => EligibleRoles.Contains(role);
}
