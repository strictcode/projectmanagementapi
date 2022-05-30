using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data.Issues;
using ProjectManagement.Database;
using ProjectManagement.Models.Issues;
using ProjectManagement.Utilities;

namespace ProjectManagement.Controllers;
[Route("api/[controller]")]
[ApiController]
public class IssueController : ControllerBase
{
    private readonly IClock clock;
    private readonly AppDbContext dbContext;

    public IssueController(
        IClock clock,
        AppDbContext dbContext
        )
    {
        this.clock = clock;
        this.dbContext = dbContext;
    }

    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IssueModel>> Get(
        [FromRoute] Guid id)
    {
        var issue = await dbContext.Issues
            .SingleOrDefaultAsync(x => x.Id == id);

        if (issue is null)
        {
            return NotFound();
        }
        return Ok(issue.ToModel());
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
        var issues = await dbContext.Issues.ToListAsync();

        return Ok(issues.Select(x => x.ToModel()));
    }

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

        }.SetCreateBy(User.GetName(), clock.GetCurrentInstant());

        dbContext.Add(issue);
        await dbContext.SaveChangesAsync();

        return Ok(issue.ToModel());
    }

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
            .SingleOrDefaultAsync(x => x.Id == id);

        if (issue is null)
        {
            return NotFound();
        }

        issue.Summary = model.Summary;
        issue.AssigneeId = model.AssigneeId;

        issue.SetModifyBy(User.GetName(), clock.GetCurrentInstant());

        await dbContext.SaveChangesAsync();

        return Ok(issue.ToModel());
    }
}
