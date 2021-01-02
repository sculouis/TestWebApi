using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestWebApi.Models;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoItemsController(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // GET: api/TodoItems
    [HttpGet]
    public async Task<ActionResult> GetTodoItems()
    {
        var connStr = Configuration.GetConnectionString("TodoItems");
        // Query Model
        var conn = new SqlConnection(connStr);
        var sql = "SELECT * FROM TodoItem";
        IEnumerable<TodoItem> results = await conn.QueryAsync<TodoItem>(sql);
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            return NotFound();
        }

        return ItemToDTO(todoItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
    {
        if (id != todoItemDTO.Id)
        {
            return BadRequest();
        }

        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        todoItem.Name = todoItemDTO.Name;
        todoItem.IsComplete = todoItemDTO.IsComplete;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult> PostTodoItem(TodoItemDTO todoItemDTO)
    {
        var todoItem = new TodoItem
        {
            Id = todoItemDTO.Id,
            IsComplete = todoItemDTO.IsComplete,
            Name = todoItemDTO.Name,

        };

        var connStr = Configuration.GetConnectionString("TodoItems");
        // Query Model
        var conn = new SqlConnection(connStr);
        conn.Execute("insert into TodoItem(Id,Name,IsComplete) values(@Id,@Name,@IsComplete)", new {todoItem.Id,todoItem.Name,todoItem.IsComplete});

        return Ok();
            }

    [HttpPost]
    [Route("another")]
    public async Task<ActionResult<TodoItemDTO>> PostTodoItemOther(TodoItemDTO todoItemDTO)
    {
        var todoItem = new TodoItem
        {
            Id = todoItemDTO.Id,
            IsComplete = todoItemDTO.IsComplete,
            Name = todoItemDTO.Name,

        };

        var connStr = Configuration.GetConnectionString("TodoItems");
        // Query Model
        var conn = new SqlConnection(connStr);
        conn.Execute("insert into TodoItem(Id,Name,IsComplete) values(@Id,@Name,@IsComplete)", new { todoItem.Id, todoItem.Name, todoItem.IsComplete });

        return CreatedAtAction(
            nameof(GetTodoItem),
            new { id = todoItem.Id },
            ItemToDTO(todoItem));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(long id) =>
         _context.TodoItems.Any(e => e.Id == id);

    private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
        new TodoItemDTO
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };
}