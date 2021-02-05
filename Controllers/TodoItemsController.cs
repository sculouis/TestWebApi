using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestWebApi.Models;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{

    public TodoItemsController(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // GET: api/TodoItems
    [HttpGet]
    public async Task<ActionResult> GetTodoItems()
    {
        var connStr = Configuration.GetConnectionString("testdb");
        // Query Model
        var conn = new MySqlConnection(connStr);
        var sql = "SELECT * FROM TodoItem";
        IEnumerable<TodoItem> results = await conn.QueryAsync<TodoItem>(sql);
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
    {
        return  NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
    {
         return  NoContent();
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
        var conn = new MySqlConnection(connStr);
        await conn.ExecuteAsync("insert into TodoItem(Id,Name,IsComplete) values(@Id,@Name,@IsComplete)", new {todoItem.Id,todoItem.Name,todoItem.IsComplete});
        return Created("",todoItem);
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
        var conn = new MySqlConnection(connStr);
        await conn.ExecuteAsync("insert into TodoItem(Id,Name,IsComplete) values(@Id,@Name,@IsComplete)", new { todoItem.Id, todoItem.Name, todoItem.IsComplete });

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(int id)
    {

        var connStr = Configuration.GetConnectionString("TodoItems");
        // Query Model
        var conn = new MySqlConnection(connStr);
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id, DbType.Int16, ParameterDirection.Input);

        await conn.ExecuteAsync("uspGetDelete", param:parameters);

        return Ok();
    }

    private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
        new TodoItemDTO
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };
}