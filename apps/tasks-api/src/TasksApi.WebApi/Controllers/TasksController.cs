using Microsoft.AspNetCore.Mvc;
using TasksApi.Application.Commands;
using TasksApi.Application.Queries;
using TasksApi.Domain.Entities;
using TasksApi.WebApi.DTOs;

namespace TasksApi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly GetAllTasksQueryHandler _queryHandler;
    private readonly ICreateTaskCommandHandler _createCommandHandler;

    public TasksController(
        GetAllTasksQueryHandler queryHandler, 
        ICreateTaskCommandHandler createCommandHandler)
    {
        _queryHandler = queryHandler;
        _createCommandHandler = createCommandHandler;
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

    [HttpPost]
    public async Task<ActionResult<TaskEntity>> Create([FromBody] CreateTaskRequest request)
    {
        try
        {
            var command = new CreateTaskCommand
            {
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                DueDate = request.DueDate,
                UserId = request.UserId
            };

            var createdTask = await _createCommandHandler.Handle(command, CancellationToken.None);
            
            return CreatedAtAction(
                nameof(GetTaskById),
                new { id = createdTask.Id },
                createdTask
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskEntity>> GetTaskById(Guid id)
    {
        // Placeholder para cumplir con CreatedAtAction
        // Se implementará en US-06
        return NotFound();
    }
}
