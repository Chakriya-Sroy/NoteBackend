using System.Text.Json.Serialization;
namespace NoteBackend.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }

   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }

}
