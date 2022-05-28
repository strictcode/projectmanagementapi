namespace ProjectManagement.Utilities;

public class Policies
{    /// <summary>
     /// Only superadmin can access this endpoint. Works as `[Authorize(Policy = SUPERADMIN)]`.
     /// </summary>
    public const string SUPERADMIN = nameof(SUPERADMIN);
    public const string USER = nameof(USER);
}
