namespace api.Params.Orders
{
    public class OrderFwdParams: PaginationParams
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public int OrderItemId { get; set; }
        public int ProfessionId { get; set; }
        public DateTime? FwdDateFrom { get; set; }
        public DateTime FwdDateUpto { get; set; } 
        public int CustomerId { get; set; }
    }
}