using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data.Issues;
using ProjectManagement.Database;
using ProjectManagement.Models.Issues;
using ProjectManagement.Utilities;

namespace ProjectManagement.Controllers;
/// <summary>
/// 
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class IssueController : ControllerBase
{
    private readonly IClock clock;
    private readonly AppDbContext dbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clock"></param>
    /// <param name="dbContext"></param>
    public IssueController(
        IClock clock,
        AppDbContext dbContext
        )
    {
        this.clock = clock;
        this.dbContext = dbContext;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IssueModel>> Get(
        [FromRoute] Guid id)
    {
        var issue = await dbContext
            .Issues
            .IncludeForDetail()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (issue is null)
        {
            return NotFound();
        }
        return Ok(issue.ToDetailModel());
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<IssueModel>>> GetList()
    {
        var issues = await dbContext
            .Issues
            .IncludeForDetail()
            .ToListAsync();

        return Ok(issues.Select(x => x.ToDetailModel()));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IssueModel>> Create(
        [FromBody] IssueCreateModel model)
    {
        if (model.AssigneeId != null)
        {
            var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == model.AssigneeId);

            if (user is null)
            {
                ModelState.AddModelError(nameof(model.AssigneeId).ToLowerInvariant(), "User does not exist");
                return ValidationProblem(ModelState);
            }
        }

        var issue = new Issue {
            Summary = model.Summary,
            AssigneeId = model.AssigneeId,
            ReporterId = User.GetUserId(),
            ProjectId = model.ProjectId,
            Status = (IssueState)model.StatusId,
        }.SetCreateBy(User.GetName(), clock.GetCurrentInstant());

        dbContext.Add(issue);
        await dbContext.SaveChangesAsync();

        issue = await dbContext.Issues.IncludeForDetail().SingleAsync(x => x.Id == issue.Id);

        return Ok(issue.ToDetailModel());
    }

    /// <summary>
    /// Todo = 1,
    /// InProgress = 2,
    /// Done = 3,
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPatch("{id}/Status/{statusId}")]
    public async Task<ActionResult<IssueModel>> UpdateStatus(
        [FromRoute] Guid id,
        [FromRoute] int statusId)
    {
        var issue = await dbContext.Issues
            .IncludeForDetail()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (issue is null)
        {
            return NotFound();
        }
        issue.Status = (IssueState)statusId;

        issue.SetModifyBy(User.GetName(), clock.GetCurrentInstant());

        await dbContext.SaveChangesAsync();

        return Ok(issue.ToDetailModel());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<IssueModel>> Update(
        [FromRoute] Guid id,
        [FromBody]  IssueCreateModel model)
    {
        if (model.AssigneeId != null)
        {
            var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == model.AssigneeId);

            if (user is null)
            {
                ModelState.AddModelError(nameof(model.AssigneeId).ToLowerInvariant(), "User does not exist");
                return ValidationProblem(ModelState);
            }
        }

        var issue = await dbContext.Issues
            .IncludeForDetail()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (issue is null)
        {
            return NotFound();
        }

        issue.Summary = model.Summary;
        issue.Description = model.Description;
        issue.AssigneeId = model.AssigneeId;
        issue.Status = (IssueState)model.StatusId;

        issue.SetModifyBy(User.GetName(), clock.GetCurrentInstant());

        await dbContext.SaveChangesAsync();

        return Ok(issue.ToDetailModel());
    }
}
