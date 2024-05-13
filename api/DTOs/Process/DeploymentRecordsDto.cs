namespace api.DTOs.Process
{
    public class DeploymentRecordsDto
    {
        public int DepId { get; set; }
        public int CVRefId { get; set; }
        public string CustomerName { get; set; }
        public string CategoryName { get; set; }
        public int OrderNo { get; set; }
        public int ApplicationNo {get; set;}
        public string CandidateName { get; set; }
        public DateOnly TransactionDate { get; set; }
        public string Status { get; set; }
        public DateOnly StatusDate { get; set; }
    }
}