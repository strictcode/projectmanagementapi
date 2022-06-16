using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data.Projects;
using ProjectManagement.Models.Issues;

namespace ProjectManagement.Models.Projects;
/// <summary>
/// 
/// </summary>
public class ProjectModel
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<IssueModel> Issues { get; set; } = Enumerable.Empty<IssueModel>();

    /// <summary>
    /// 
    /// </summary>
    public string CreatedName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string CreatedDate { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string ModifiedName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string ModifiedDate { get; set; } = string.Empty;
}

/// <summary>
/// 
/// </summary>
public static class ProjectModelExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="queryable"></param>
    /// <returns></returns>
    public static IQueryable<Project> IncludeForModel(this IQueryable<Project> queryable)
        => queryable
        .Include(x => x.Issues)
            .ThenInclude(x => x.Assignee)
        .Include(x => x.Issues)
            .ThenInclude(x => x.Reporter)
        .Include(x => x.Issues)
        ;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ProjectModel ToModel(this Project source)
        => new() {
            Id = source.Id,
            Name = source.Name,
            Issues = source.Issues.Select(x => x.ToDetailModel()),
            CreatedDate = source.CreatedTimestamp.ToString(),
            ModifiedDate = source.ModifiedTimestamp.ToString(),
            CreatedName = source.CreatedBy,
            ModifiedName = source.ModifiedBy,
        };
}
