namespace ProjectManagement.Models.Users;

public class LoggedUserModel
{
    public string UserName { get; set; } = null!;

    public bool IsAuthenticated { get; set; }
}
