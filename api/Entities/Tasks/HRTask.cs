using System.ComponentModel.DataAnnotations;

namespace api.Entities.Tasks
{
    public class HRTask: BaseEntity
    {
        public DateTime TaskDate { get; set; } =DateTime.UtcNow;
        [Required]
        public string AssignedByUsername {get; set;}
        [Required]
        public string AssignedToUsername {get; set;}
        public int OrderId {get; set;}
        public int OrderNo { get; set; }
        public int OrderItemId {get; set;}
        public int QntyAssigned {get; set;}
        public int QntyDelivered {get; set;}
        [Required]
        public string TaskDescription {get; set;}
        [Required]
        public DateTime CompleteBy {get; set;}
        public DateTime? TaskStatusDate { get; set; }
        public string StatusUpdatedBy {get; set;}
        [Required]
        public string TaskStatus {get; set;}="Not Started";
        
        public virtual ICollection<HRTaskItem> HRTaskItems {get; set;}
    }
}