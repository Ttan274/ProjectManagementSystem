using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.SubTask
{
    public class CreateSubTaskRequestModel
    {
        public CreateSubTaskRequestModel()
        {
            
        }
        [Required]
        public string Description { get; set; }
    }
}
