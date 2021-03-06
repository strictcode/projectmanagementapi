using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data.Projects;
using ProjectManagement.Database;
using ProjectManagement.Models.Projects;
using ProjectManagement.Utilities;

namespace ProjectManagement.Controllers;
/// <summary>
/// 
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IClock clock;
    private readonly AppDbContext dbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clock"></param>
    /// <param name="dbContext"></param>
    public ProjectController(
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
    public async Task<ActionResult<ProjectModel>> Get(
        [FromRoute] Guid id)
    {
        var project = await dbContext.Projects
            .IncludeForModel()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (project is null)
        {
            return NotFound();
        }
        return Ok(project.ToModel());
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProjectModel>>> GetList()
    {
        var projects = await dbContext
            .Projects
            .IncludeForModel()
            .ToListAsync();

        return Ok(projects.Select(x => x.ToModel()));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProjectModel>> Create(
        [FromBody] ProjectCreateModel model)
    {
        var project = new Project {
            Name = model.Name,
            OwnerId = User.GetUserId(),
        }.SetCreateBy(User.GetName(), clock.GetCurrentInstant());

        dbContext.Add(project);
        await dbContext.SaveChangesAsync();

        project = await dbContext.Projects.IncludeForModel().SingleAsync(x => x.Id == project.Id);

        return Ok(project.ToModel());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectModel>> Update(
        [FromRoute] Guid id,
        [FromBody] ProjectCreateModel model)
    {
        var project = await dbContext
            .Projects
            .IncludeForModel()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (project is null)
        {
            return NotFound();
        }

        project.Name = model.Name;

        project.SetModifyBy(User.GetName(), clock.GetCurrentInstant());

        await dbContext.SaveChangesAsync();

        return Ok(project.ToModel());
    }
}
