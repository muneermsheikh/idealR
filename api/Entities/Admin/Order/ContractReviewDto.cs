namespace api.Entities.Admin.Order
{
    public class ContractReviewDto
    {
        public int OrderNo {get; set; }
        public DateOnly OrderDate {get; set;}
        public string CustomerName {get; set;}
        public string ReviewedByName { get; set; }
        public DateTime ReviewedOn { get; set; } = DateTime.Now;
        public string ReviewStatus { get; set; } = "NotReviewed";
        public bool ReleasedForProduction { get; set; }=false;
    }
}