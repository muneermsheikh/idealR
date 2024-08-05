namespace api.DTOs.Admin
{
    public class TaskInBriefDto
    {
        public int Id { get; set; }
        public int Identity { get; set; }
        public string TaskType { get; set; }
        public DateTime TaskDate { get; set; }
        public string TaskOwnerUsername { get; set; }
        public string AssignedToUsername { get; set; }
        public int OrderNo { get; set; }
        public int ApplicationNo { get; set; }
        public string TaskDescription { get; set; }
        public DateTime CompleteBy { get; set; }    
        public string TaskStatus { get; set; }
    }
}