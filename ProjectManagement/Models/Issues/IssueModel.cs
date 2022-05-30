using ProjectManagement.Data.Issues;

namespace ProjectManagement.Models.Issues;

public class IssueModel
{
    public Guid Id { get; set; }

    public string Summary { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public Guid ReporterId { get; set; }

    public Guid? AssigneeId { get; set; }
}
public static class IssueModelExtensions
{
    public static IssueModel ToModel(this Issue source)
        => new() {
            Id = source.Id,
            Summary = source.Summary,
            Description = source.Description,
        };
}
