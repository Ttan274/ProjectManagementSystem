namespace ProjectManagementSystem.Domain.Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public virtual bool Status { get; set; } = true;
        public virtual DateTime CreatedDatee { get; set; } = DateTime.Now;
        public virtual string? Tag { get; set; }
    }
}
