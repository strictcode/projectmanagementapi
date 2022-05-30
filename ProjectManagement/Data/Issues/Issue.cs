using ProjectManagement.Data.Identity;
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

    public string Description { get; set; } = string.Empty;

    public User Reporter { get; set; } = null!;
    public Guid ReporterId { get; set; }

    public User? Assignee { get; set; }
    public Guid? AssigneeId { get; set; }

    public Instant CreatedTimestamp { get; set; }

    public string CreatedBy { get; set; } = null!;

    public Instant ModifiedTimestamp { get; set; }

    public string ModifiedBy { get; set; } = null!;
}
