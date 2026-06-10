using Microsoft.AspNetCore.Mvc;
using TasksApi.Application.Queries;
using TasksApi.Domain.Entities;

namespace TasksApi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly GetAllTasksQueryHandler _queryHandler;

    public TasksController(GetAllTasksQueryHandler queryHandler)
    {
        _queryHandler = queryHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskEntity>>> GetAllTasks([FromQuery] Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(new { message = "UserId is required and cannot be empty" });
        }

        try
        {
            var query = new GetAllTasksQuery(userId);
            var tasks = await _queryHandler.Handle(query, CancellationToken.None);
            return Ok(tasks);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
