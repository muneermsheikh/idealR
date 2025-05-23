namespace api.DTOs.Admin.Orders
{
    public class OpenOrderItemCategoriesDto
    {
        public bool Checked { get; set; }
        //public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public int OrderItemId { get; set; }
        //public int ProfessionId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string CategoryRefAndName { get; set; }
        public string RequireInternalReview { get; set; }
        public bool AssessmentQDesigned { get; set; }
        public int Quantity { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
    }
}