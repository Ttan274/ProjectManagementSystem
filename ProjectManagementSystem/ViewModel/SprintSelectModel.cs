namespace ProjectManagementSystem.ViewModel
{
    public class SprintSelectModel
    {
        public SprintSelectModel()
        {

        }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int IsCurrent { get; set; }
    }
}
