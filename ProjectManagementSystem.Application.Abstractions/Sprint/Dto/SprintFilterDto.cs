namespace ProjectManagementSystem.Application.Abstractions.Sprint.Dto
{
    public class SprintFilterDto
    {
        public SprintFilterDto()
        {

        }
        public Guid? Id { get; set; }
        public Guid? ProjectId { get; set; }

        public void Validate(out string validationMessage)
        {
            List<string> errors = [];
            if (Id == null)
            {
                errors.Add("Sprint is required");
            }
            if (ProjectId == null)
            {
                errors.Add("Project is required");
            }

            if (errors.Count > 0)
            {
                validationMessage = string.Join(". ", errors);
            }
            else
            {
                validationMessage = string.Empty;
            }
        }
    }
}
