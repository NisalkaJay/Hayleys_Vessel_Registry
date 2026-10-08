namespace VesselRegistry.Api.Dtos
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }

        public static ApiResponse<T> Ok(T data) => 
            new() { Success = true, Data = data };

        public static ApiResponse<T> Error(string errorCode, string message, T? validationErrors = default) => 
            new() { Success = false, ErrorCode = errorCode, Message = message, Data = validationErrors };
    }
}