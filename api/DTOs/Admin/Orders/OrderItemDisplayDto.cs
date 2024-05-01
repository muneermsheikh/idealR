using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    public class OrderItemDisplayDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int SrNo { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public string SourceFrom { get; set; }
        public int Quantity { get; set; }
        public int MinCVs { get; set; }
        public int MaxCVs { get; set; }
        public bool Ecnr { get; set; }=false;
        public DateOnly CompleteBefore { get; set; }
        public string Status { get; set; }="Not Started";
        public bool Checked {get; set;}
        public string ReviewItemStatus { get; set; }="NotReviewed";
       
    }
}