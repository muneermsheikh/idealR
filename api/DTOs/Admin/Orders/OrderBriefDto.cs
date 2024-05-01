namespace api.DTOs.Admin.Orders
{
    public class OrderBriefDto
    {
        public int Id { get; set; }
        public int OrderNo { get; set; }
        public DateOnly OrderDate { get; set; }
        public string CustomerName { get; set; } 
        public string Status { get; set; }
        public DateOnly ContractReviewedOn { get; set; }
        public DateOnly CompleteBy { get; set; }
    }

    
}