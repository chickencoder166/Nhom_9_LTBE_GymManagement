namespace QLPG_a.Services
{
    public enum ServiceErrorCode
    {
        None = 0,
        NotFound,
        Validation,
        Conflict,
        Concurrency,
        Unexpected
    }

    public class ServiceResult
    {
        public bool Succeeded { get; }
        public ServiceErrorCode ErrorCode { get; }
        public string? ErrorMessage { get; }

        protected ServiceResult(bool succeeded, ServiceErrorCode errorCode, string? errorMessage)
        {
            Succeeded = succeeded;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public static ServiceResult Success() => new ServiceResult(true, ServiceErrorCode.None, null);
        public static ServiceResult Fail(ServiceErrorCode code, string message) => new ServiceResult(false, code, message);
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Value { get; }

        private ServiceResult(bool succeeded, ServiceErrorCode errorCode, string? errorMessage, T? value)
            : base(succeeded, errorCode, errorMessage)
        {
            Value = value;
        }

        public static ServiceResult<T> Success(T value) => new ServiceResult<T>(true, ServiceErrorCode.None, null, value);
        public static new ServiceResult<T> Fail(ServiceErrorCode code, string message) => new ServiceResult<T>(false, code, message, default);
    }
}
