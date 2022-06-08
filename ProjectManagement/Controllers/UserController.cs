using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data.Identity;
using ProjectManagement.Database;
using ProjectManagement.Models.Users;
using ProjectManagement.Utilities;

namespace ProjectManagement.Controllers;
/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly ILogger<UserController> logger;
    private readonly AppDbContext dbContext;
    private readonly IClock clock;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="clock"></param>
    public UserController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserController> logger,
        AppDbContext dbContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IClock clock
        )
    {
        this.httpContextAccessor = httpContextAccessor;
        this.logger = logger;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.dbContext = dbContext;
        this.clock = clock;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("Login")]
    public async Task<ActionResult> LoginAsync(
        [FromBody] LoginModel model
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("Register")]
    public async Task<ActionResult> Register(
        [FromBody] RegisterModel model
        )
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            ModelState.AddModelError(string.Empty, "MAIL_TAKEN");
            return ValidationProblem(ModelState);
        }

        user = new User {
            UserName = model.Email,
            Email = model.Email,
        }.SetCreateBySystem(clock.GetCurrentInstant());

        {
            var result = await userManager.AddPasswordAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "PASSWORD_ERROR");
            }
        }
        { 
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "CREATE_ERROR");
            }
        }
        return NoContent();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> GetList()
    {
        var users = await dbContext.Users.ToListAsync();
        return Ok(users.Select(x => x.Email));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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
