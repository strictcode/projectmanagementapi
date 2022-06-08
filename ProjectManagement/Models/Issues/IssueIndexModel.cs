using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data.Issues;
using ProjectManagement.Models.Users;

namespace ProjectManagement.Models.Issues;
/// <summary>
/// 
/// </summary>
public class IssueIndexModel
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
    public UserShortModel? Assignee { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ProjectName { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public static class IssueINdexModelExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="queryable"></param>
    /// <returns></returns>
    public static IQueryable<Issue> IncludeForIndex(this IQueryable<Issue> queryable)
        => queryable
        .Include(x => x.Assignee)
        .Include(x => x.Project)
        ;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IssueIndexModel ToIndexModel(this Issue source)
        => new() {
            Id = source.Id,
            Summary = source.Summary,
            Assignee = source.Assignee?.ToShortModel(),
            ProjectName = source.Project.Name,
        };
}
