namespace api.DTOs.Admin.Orders
{
    public class OrderBriefDto
    {
        public int Id { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } 
        public string CityOfWorking { get; set; }
        public string Status { get; set; }
        public string ContractReviewStatus { get; set; }
        public int ContractReviewId { get; set; }
        public DateTime? ContractReviewedOn { get; set; }
        public DateTime? ForwardedToHRDeptOn { get; set; }
        public DateTime? AcknowledgedToClientOn { get; set; }
        public DateTime CompleteBy { get; set; }
    }

    
}