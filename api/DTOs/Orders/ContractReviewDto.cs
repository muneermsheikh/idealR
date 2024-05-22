namespace api.DTOs.Order
{
    public class ContractReviewDto
    {
        public int Id { get; set; }
        public int OrderNo {get; set; }
        public DateOnly OrderDate {get; set;}
        public string CustomerName {get; set;}
        public string ReviewedByName { get; set; }
        public DateTime ReviewedOn { get; set; } 
        public string ReviewStatus { get; set; } 
        public bool ReleasedForProduction { get; set; }
    }
}