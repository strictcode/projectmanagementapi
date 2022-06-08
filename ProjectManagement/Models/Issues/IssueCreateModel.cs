using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models.Issues;
/// <summary>
/// 
/// </summary>
public class IssueCreateModel
{
    /// <summary>
    /// 
    /// </summary>
    [Required(ErrorMessage = "{0} is required.", AllowEmptyStrings = false)]
    public string Summary { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid? AssigneeId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid ProjectId { get; set; }
}
