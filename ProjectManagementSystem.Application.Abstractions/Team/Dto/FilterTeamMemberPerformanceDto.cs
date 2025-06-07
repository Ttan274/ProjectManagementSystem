namespace ProjectManagementSystem.Application.Abstractions.Team.Dto
{
    public class FilterTeamMemberPerformanceDto
    {
        public FilterTeamMemberPerformanceDto()
        {

        }

        public Guid TeamId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public void Validate(out string validationResult)
        {
            if (TeamId == Guid.Empty)
            {
                validationResult = "Team not found";
                return;
            }
            else if (StartDate == DateTime.MinValue)
            {
                validationResult = "Start date required";
                return;
            }
            else if (EndDate == DateTime.MinValue)
            {
                validationResult = "End date required";
                return;
            }

            validationResult = string.Empty;
        }
    }
}
