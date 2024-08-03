namespace api.Params.Admin
{
    public class TaskParams: PaginationParams
    {
        public int TaskId { get; set; }
        public string TaskType { get; set; }
        public DateTime TaskDate { get; set; }
        public int candidateId { get; set; }
        public int ApplicationNo { get; set; }
        public string ResumeId { get; set; }   
        public string AssignedToUserName { get; set; }
        public string AssignedByUsername { get; set; }
        public string TaskStatus { get; set; }
        public int OrderId { get; set; }

    }
}