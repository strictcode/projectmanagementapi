using ProjectManagement.Data.Identity;

namespace ProjectManagement.Utilities;

public class Claims
{
    public const string SUPERUSER = User.Entry.ClaimTypeSuperUser;
    public const string USER = "CLAIM_USER";
}
