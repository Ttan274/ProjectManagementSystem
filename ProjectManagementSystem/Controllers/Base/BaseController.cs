using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Controllers.Base
{
    public class BaseController : Controller
    {
        protected ServiceResponse<T> Success<T>(T data, string? message = null)
        {
            return new ServiceResponse<T> { Success = true, Data = data, Message = message };
        }

        protected ServiceResponse<T> Error<T>(T data, string errorMessage)
        {
            return new ServiceResponse<T> { Success = false, Data = data, ErrorMessage = errorMessage };
        }

        protected ServiceResponse<object> Error(string errorMessage)
        {
            return new ServiceResponse<object> { Success = false, Data = null, ErrorMessage = errorMessage };
        }
    }
}
