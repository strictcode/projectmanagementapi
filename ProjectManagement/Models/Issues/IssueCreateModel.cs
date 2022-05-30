using System.ComponentModel.DataAnnotations;
using ProjectManagement.Data.Projects;

namespace ProjectManagement.Models.Issues;
public class IssueCreateModel
{
    [Required(ErrorMessage = "{0} is required.", AllowEmptyStrings = false)]
    public string Summary { get; set; } = null!;

    public Guid? AssigneeId { get; set; }
}
