using System.ComponentModel.DataAnnotations;

namespace api.Entities.Tasks
{
    public class TaskItem: BaseEntity
    {
        [Required]
        public int AppTaskId { get; set; }
        [Required]
        public DateOnly TransactionDate { get; set; }
        [Required]
        public string TaskItemDescription {get; set;}
        [Required]
        public string UserName {get; set;}
        public DateOnly? NextFollowupOn {get; set;}
        public string NextFollowupByName {get; set;}
    }
}