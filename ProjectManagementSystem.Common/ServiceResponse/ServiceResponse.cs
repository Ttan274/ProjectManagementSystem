namespace ProjectManagementSystem.Common.ServiceResponse
{
    public class ServiceResponse<T>
    {
        public ServiceResponse()
        {

        }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }
    }
}
