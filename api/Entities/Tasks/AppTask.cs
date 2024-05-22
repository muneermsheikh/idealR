using System.ComponentModel.DataAnnotations;

namespace api.Entities.Tasks
{
    public class AppTask: BaseEntity
    {
        public AppTask()
        {
        }

        public int? CVRefId { get; set; }
        public string TaskType { get; set; }
        public int? CandidateAssessmentId { get; set; }
        public DateOnly TaskDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Required]
        public string TaskOwnerUsername {get; set;}
        [Required]
        public string AssignedToUsername {get; set;}
        public int? OrderId {get; set;}
        public int? OrderNo { get; set; }
        public int OrderItemId {get; set;}
        public int? ApplicationNo { get; set; }
        [MaxLength(15)]
        public string ResumeId {get; set;}
        public int CandidateId { get; set; }
        [Required]
        public string TaskDescription {get; set;}
        [Required]
        public DateOnly CompleteBy {get; set;}
        [Required]
        public string TaskStatus {get; set;}="Not Started";
        public DateOnly? CompletedOn { get; set; }
        public int? HistoryItemId { get; set; }
        public string PostTaskAction { get; set; } ="Do Not Compose Message";
        public virtual ICollection<TaskItem> TaskItems {get; set;}

    }
}