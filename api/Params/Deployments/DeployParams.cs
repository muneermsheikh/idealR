namespace api.Params.Deployments
{
    public class DeployParams: PaginationParams
    {
        public int CvRefId { get; set; }
        public int OrderItemId { get; set; }
        public int CandidateId {get; set;}
        public DateTime SelectedOn {get; set;}
        public int OrderNo {get; set;}
        public int CustomerId { get; set; }
        public string Status {get; set;}
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string CategoryName { get; set; }
        public string CustomerName { get; set; }
        public string CurrentStatus { get; set; }
    }
}