using Microsoft.AspNetCore.Mvc;
using TasksApi.Application.Commands;
using TasksApi.Application.Exceptions;
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
    private readonly IUpdateTaskCommandHandler _updateCommandHandler;
    private readonly IDeleteTaskCommandHandler _deleteCommandHandler;

    public TasksController(
        GetAllTasksQueryHandler queryHandler, 
        ICreateTaskCommandHandler createCommandHandler,
        IUpdateTaskCommandHandler updateCommandHandler,
        IDeleteTaskCommandHandler deleteCommandHandler)
    {
        _queryHandler = queryHandler;
        _createCommandHandler = createCommandHandler;
        _updateCommandHandler = updateCommandHandler;
        _deleteCommandHandler = deleteCommandHandler;
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

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskEntity>> UpdateTask(Guid id, [FromBody] UpdateTaskRequest request, [FromQuery] Guid userId)
    {
        try
        {
            var command = new UpdateTaskCommand
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                Status = request.Status,
                DueDate = request.DueDate,
                UserId = userId
            };

            var updatedTask = await _updateCommandHandler.Handle(command, CancellationToken.None);
            
            return Ok(updatedTask);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(Guid id, [FromQuery] Guid userId)
    {
        try
        {
            var command = new DeleteTaskCommand
            {
                Id = id,
                UserId = userId
            };

            await _deleteCommandHandler.Handle(command, CancellationToken.None);
            
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }
}
