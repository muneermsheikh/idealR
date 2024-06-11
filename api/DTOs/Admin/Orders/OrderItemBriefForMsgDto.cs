using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    //change this to: OrderHeader + collection of items
    public class OrderItemBriefForMsgDto
    {
        public int OrderId { get; set; }
       
        
        public int OrderItemId { get; set; }
        public int SrNo { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int Quantity { get; set; }
        public bool Ecnr { get; set; }=false;
        public DateTime CompleteBefore { get; set; }
        public JobDescription JobDescription { get; set; }
        public Remuneration Remuneration { get; set; }
        public string Status { get; set; }
        public bool Checked {get; set;}
    }

    
}