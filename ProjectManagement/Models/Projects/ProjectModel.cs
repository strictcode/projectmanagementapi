using ProjectManagement.Data.Projects;

namespace ProjectManagement.Models.Projects;
public class ProjectModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatedName { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public string ModifiedName { get; set; } = string.Empty;
    public string ModifiedDate { get; set; } = string.Empty;
}

public static class ProjectModelExtensions
{
    public static ProjectModel ToModel(this Project source)
        => new() {
            Id = source.Id,
            Name = source.Name,
            CreatedDate = source.CreatedTimestamp.ToString(),
            ModifiedDate = source.ModifiedTimestamp.ToString(),
            CreatedName = source.CreatedBy,
            ModifiedName = source.ModifiedBy,
        };
}
