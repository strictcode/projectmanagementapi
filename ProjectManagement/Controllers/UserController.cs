using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data.Identity;
using ProjectManagement.Models.Users;
using ProjectManagement.Utilities;

namespace ProjectManagement.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly ILogger<UserController> logger;
    private readonly AppDbContext dbContext;

    public UserController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserController> logger,
        AppDbContext dbContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager
        )
    {
        this.httpContextAccessor = httpContextAccessor;
        this.logger = logger;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.dbContext = dbContext;
    }

    [Authorize]
    [HttpGet("UserInfo")]
    public async Task<ActionResult<LoggedUserModel>> GetUserInfo()
    {
        var principal = GetUserPrincipalFromContext();

        if (!principal.Identities.Any(x => x.IsAuthenticated))
        {
            return ValidationProblem(new ValidationProblemDetails { Detail = "Something went wrong!", Status = StatusCodes.Status400BadRequest });
        }

        var id = TryGetUserIdFromContext();
        var user = await userManager.Users
            .SingleAsync(x => x.Id == id);

        return Ok(new LoggedUserModel {
            UserName = user.UserName,
            IsAuthenticated = true,
        });
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("Login")]
    public async Task<ActionResult> LoginAsync(
        LoginModel model
        )
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "LOGIN_FAILED");
            return ValidationProblem(ModelState);
        }

        // Try to use crendetials and log user in
        var signInResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, true);
        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "LOGIN_FAILED");
            return ValidationProblem(ModelState);
        }
        var userPrincipal = await signInManager.CreateUserPrincipalAsync(user);
        await HttpContext.SignInAsync(userPrincipal);

        return NoContent();
    }

    [Authorize]
    [HttpPost("Logout")]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return NoContent();
    }

    private ClaimsPrincipal GetUserPrincipalFromContext()
    {
        _ = httpContextAccessor.HttpContext ?? throw new ArgumentNullException("HttpContextAccessor.HttpContext");
        return httpContextAccessor.HttpContext.User;
    }

    private Guid? TryGetUserIdFromContext()
    {
        var user = GetUserPrincipalFromContext();
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            return null;
        }
        return user.GetUserId();
    }
}
