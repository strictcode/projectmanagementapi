using System.ComponentModel.DataAnnotations;
using ProjectManagement.Data.Projects;

namespace ProjectManagement.Models.Projects;
public class ProjectCreateModel
{
    [Required(ErrorMessage = "{0} is required.", AllowEmptyStrings = false)]
    [MaxLength(Project.Metadata.NameLength)]
    public string Name { get; set; } = null!;
}
