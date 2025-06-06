using Microsoft.Extensions.Logging;

namespace ProjectManagementSystem.Common.ServiceResponse
{
    public class ServiceResponseHelper(ILogger<ServiceResponseHelper> logger) : IServiceResponseHelper
    {
        public ServiceResponse<T> SetSuccess<T>(T data, string? message = null)
        {
            return new ServiceResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                ErrorMessage = null
            };
        }

        public ServiceResponse<T> SetError<T>(string errorMessage, bool? isLogging = false)
        {
            if (isLogging == true)
            {
                logger.LogError("Error occured : {ErrorMessage}", errorMessage);
            }

            return new ServiceResponse<T>
            {
                Success = false,
                Data = default,
                Message = string.Empty,
                ErrorMessage = errorMessage
            };
        }

        public ServiceResponse<T> SetError<T>(T data, string errorMessage, bool? isLogging = false)
        {
            if (isLogging == true)
            {
                logger.LogError("Error occured : {ErrorMessage}", errorMessage);
            }

            return new ServiceResponse<T>
            {
                Success = false,
                Data = data,
                Message = string.Empty,
                ErrorMessage = errorMessage
            };
        }
    }
}
