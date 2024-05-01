namespace api.DTOs.Admin.Orders
{
    public class OrderDisplayDto
    {
        public int Id { get; set; }
        public int OrderNo { get; set; }
        public DateOnly OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string OrderRef { get; set; }
        public DateOnly OrderRefDate {get; set;}
        public int ProjectManagerId { get; set; }
        public string ProjectManagerName { get; set; }
        public DateOnly CompleteBy { get; set; }
        public string Country { get; set; }
        public string CityOfWorking { get; set; }
        public int ContractReviewId { get; set; }
        public string ContractReviewStatus {get; set;}
        public string Status { get; set; } = "Awaiting Review";
        public DateTime? ForwardedToHRDeptOn { get; set; }
        public ICollection<OrderItemDisplayDto> OrderItems { get; set; }
    }
}