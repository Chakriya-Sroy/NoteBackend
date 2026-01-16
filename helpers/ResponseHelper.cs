
using NoteBackend.Models;
namespace NoteBackend.Helpers;

public static class ResponseHelper
{
    public static ApiResponse<T> Success<T>(T data, string message = "success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Success<T>(string message)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,

        };
    }

    public static ApiResponse<T> Error<T>(string message, T? data = default)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<object> Error(string message)
    {
        return new ApiResponse<object>
        {
            Success = false,
            Message = message,
        };
    }
}