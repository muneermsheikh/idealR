using System.ComponentModel.DataAnnotations;

namespace api.Entities.Tasks
{
    public class TaskItem: BaseEntity
    {
        [Required]
        public int AppTaskId { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        public string TaskItemDescription {get; set;}
        [Required]
        public string UserName {get; set;}
        public DateTime? NextFollowupOn {get; set;}
        public string NextFollowupByName {get; set;}
    }
}