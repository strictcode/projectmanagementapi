using ProjectManagement.Data.Identity;
using ProjectManagement.Data.Projects;
using ProjectManagement.Database;

namespace ProjectManagement.Data.Issues;

/// <summary>
/// 
/// </summary>
public class Issue : ITrackable
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    public string Summary { get; set; } = null!;

    public string? Description { get; set; }

    public IssueState Status { get; set; }

    public Project Project { get; set; }
    public Guid ProjectId { get; set; }

    public User Reporter { get; set; } = null!;
    public Guid ReporterId { get; set; }

    public User? Assignee { get; set; }
    public Guid? AssigneeId { get; set; }

    public Instant CreatedTimestamp { get; set; }

    public string CreatedBy { get; set; } = null!;

    public Instant ModifiedTimestamp { get; set; }

    public string ModifiedBy { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public enum IssueState
{
    /// <summary>
    /// 
    /// </summary>
    Todo = 1,
    /// <summary>
    /// 
    /// </summary>
    InProgress = 2,
    /// <summary>
    /// 
    /// </summary>
    Done = 3,
}
