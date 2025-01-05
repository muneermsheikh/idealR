namespace api.DTOs.Process
{
    public class DeploymentPendingDto
    {
        public int DepId { get; set; }
        public int ApplicationNo {get; set;}
        public string CandidateName { get; set; }
        
        public DateTime TransactionDate { get; set; }
        public int CvRefId { get; set; }
        public int OrderItemId { get; set; }
        public bool Ecnr { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string CityOfWorking { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CategoryName { get; set; }
        public DateTime ReferredOn { get; set; }
        public DateTime SelectedOn {get; set;}
        public int DeploySequence { get; set; }
        public int NextSequence { get; set; }
        public DateTime CurrentSeqDate { get; set; }
        public DateTime NextStageDate { get; set; }
        public string CurrentStatus { get; set; }  
     }
}