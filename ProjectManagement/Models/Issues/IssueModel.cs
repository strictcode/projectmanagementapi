using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data.Issues;
using ProjectManagement.Models.Users;

namespace ProjectManagement.Models.Issues;
/// <summary>
/// 
/// </summary>
public class IssueModel
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Summary { get; set; } = null!;
    /// <summary>
    /// 
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public int StatusId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public UserShortModel Reporter { get; set; } = new();
    /// <summary>
    /// 
    /// </summary>
    public UserShortModel? Assignee { get; set; }
}

/// <summary>
/// 
/// </summary>
public static class IssueModelExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="queryable"></param>
    /// <returns></returns>
    public static IQueryable<Issue> IncludeForDetail(this IQueryable<Issue> queryable)
        => queryable
        .Include(x => x.Assignee)
        .Include(x => x.Reporter)
        ;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IssueModel ToDetailModel(this Issue source)
        => new() {
            Id = source.Id,
            Summary = source.Summary,
            StatusId = (int)source.Status,
            Description = source.Description,
            Assignee = source.Assignee?.ToShortModel(),
            Reporter = source.Reporter.ToShortModel(),
        };
}
