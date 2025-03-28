namespace ProjectManagementSystem.Application.Abstractions.Base
{
    public class BaseDto
    {
        public Guid Id { get; set; }
        public virtual bool Status { get; set; } = true;
        public virtual DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
