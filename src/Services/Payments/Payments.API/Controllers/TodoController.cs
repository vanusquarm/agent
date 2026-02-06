using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.API.Entities.Models;
using Todo.API.Services;

namespace Todo.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]s")]
    //[Authorize]
    public class TodoController(ITodoClient todoClient) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var todos = await todoClient.GetTodos();
            return Ok(todos);
        }
    }
}
