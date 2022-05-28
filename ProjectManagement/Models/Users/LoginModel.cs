using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models.Users;
public class LoginModel
{
    [Required(ErrorMessage = "{0} is required.", AllowEmptyStrings = false)]
    [EmailAddress(ErrorMessage = "{0} has to be a valid value.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "{0} is required.", AllowEmptyStrings = false)]
    public string Password { get; set; } = null!;
}
