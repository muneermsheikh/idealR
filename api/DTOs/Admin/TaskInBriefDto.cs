namespace api.DTOs.Admin
{
    public class TaskInBriefDto
    {
        public int Identity { get; set; }
        public string TaskType { get; set; }
        public DateOnly TaskDate { get; set; }
        public string TaskOwnerUsername { get; set; }
        public string AssignedToUsername { get; set; }
        public int OrderNo { get; set; }
        public int ApplicationNo { get; set; }
        public string TaskDescription { get; set; }
        public DateOnly CompleteBy { get; set; }    
        public string TaskStatus { get; set; }
    }
}