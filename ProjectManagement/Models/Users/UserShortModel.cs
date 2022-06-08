using ProjectManagement.Data.Identity;

namespace ProjectManagement.Models.Users;
/// <summary>
/// 
/// </summary>
public class UserShortModel
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Mail { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public static class UserShortModelExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static UserShortModel ToShortModel(this User source)
        => new() {
            Id = source.Id,
            Mail = source.Email,
        };
}
