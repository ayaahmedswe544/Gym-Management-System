namespace Gym_Management_System.Business.GeneralResponse
{
    public class GeneralResponse<T>
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public T? Data { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }

        public static GeneralResponse<T> Ok(T data, string? message = null) => new() { Success = true, Data = data, Message = message };
        public static GeneralResponse<T> Failure(string message, Dictionary<string, string[]>? errors = null) => new() { Success = false, Message = message, Errors = errors };
    }

    public class GeneralResponse : GeneralResponse<object>
    {
        public static GeneralResponse Ok(string? message = null) => new() { Success = true, Message = message };
        public static new GeneralResponse Failure(string message, Dictionary<string, string[]>? errors = null) => new() { Success = false, Message = message, Errors = errors };
    }

}
