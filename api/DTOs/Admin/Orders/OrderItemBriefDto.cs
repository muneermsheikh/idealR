namespace api.DTOs.Admin.Orders
{
    public class OrderItemBriefDto
    {
        public int OrderId { get; set; }
        public int SrNo { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int Quantity { get; set; }
        public bool Ecnr { get; set; }=false;
        public DateOnly CompleteBefore { get; set; }
        public string Status { get; set; }
        public bool Checked {get; set;}
    }
}