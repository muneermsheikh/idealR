namespace api.DTOs.Process
{
    public class DeploymentPendingTempDto
    {
        
        public int CVRefId { get; set; }
        public int ApplicationNo {get; set;}
        public string CandidateName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int OrderNo { get; set; }
        public int OrderItemId { get; set; }
        public DateOnly OrderDate { get; set; }
        public string CategoryName { get; set; }
        public DateOnly SelectedOn {get; set;}
        public string   CurrentSeqName { get; set; }
        public DateOnly CurrentSeqDate { get; set; }
        public string NextSeqName { get; set; }
        public DateOnly NextStageDate { get; set; }
        public int DaysSinceLastSequence { get; set; }
        public int DepId { get; set; }

    }
}