using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data.Projects;
using ProjectManagement.Database;
using ProjectManagement.Models.Projects;
using ProjectManagement.Utilities;

namespace ProjectManagement.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IClock clock;
    private readonly AppDbContext dbContext;

    public ProjectController(
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
    public async Task<ActionResult<ProjectModel>> Get(
        [FromRoute] Guid id)
    {
        var project = await dbContext.Projects
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
        var projects = await dbContext.Projects.ToListAsync();

        return Ok(projects.Select(x => x.ToModel()));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProjectModel>> Create(
        [FromBody] ProjectCreateModel model)
    {
        var project = new Project {
            Name = model.Name,
        }.SetCreateBy(User.GetName(), clock.GetCurrentInstant());

        dbContext.Add(project);
        await dbContext.SaveChangesAsync();

        return Ok(project.ToModel());
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectModel>> Update(
        [FromRoute] Guid id,
        [FromBody] ProjectCreateModel model)
    {
        var project = await dbContext.Projects
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
