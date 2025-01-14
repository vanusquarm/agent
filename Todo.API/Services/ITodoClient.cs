using Refit;
using Todo.API.Entities.Models;

namespace Todo.API.Services
{
    public interface ITodoClient
    {
        [Get("/api/fruits")] //specify the finacle resource endpoint
        Task<GetTodosResponse> GetTodos();
    }

    // Move to Entities/DTOs folder
    public class GetTodosResponse
    {
        public string status { get; set; }
        public List<string> data { get; set; }
    }
}
