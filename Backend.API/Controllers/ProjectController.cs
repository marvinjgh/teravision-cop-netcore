using Backend.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IRepositoryWrapper _repository;

    public ProjectController(IRepositoryWrapper repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(long id)
    {

        var project = await _repository.Project.GetProjectById(id);

        if (project == null)
        {
            return NotFound(id);
        }

        return Ok(project);
    }

}
