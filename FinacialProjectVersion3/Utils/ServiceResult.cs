namespace FinacialProjectVersion3.Utils
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string[] Errors { get; set; }

        // Factory methods
        public static ServiceResult Succeeded(string message = null)
        {
            return new ServiceResult
            {
                Success = true,
                Message = message
            };
        }

        public static ServiceResult Failed(string message, params string[] errors)
        {
            return new ServiceResult
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// Generic ServiceResult cho phép trả về dữ liệu cùng với kết quả
    /// </summary>
    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; set; }

        // Factory methods
        public static ServiceResult<T> Succeeded(T data, string message = null)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ServiceResult<T> Failed(string message, params string[] errors)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
