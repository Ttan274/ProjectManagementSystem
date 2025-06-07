namespace ProjectManagementSystem.Common.ServiceResponse
{
    public interface IServiceResponseHelper
    {
        ServiceResponse<T> SetSuccess<T>(T data, string? message = null);
        ServiceResponse<T> SetError<T>(string errorMessage, bool? isLogging = false);
        ServiceResponse<T> SetError<T>(T data, string errorMessage, bool? isLogging = false);
    }
}
