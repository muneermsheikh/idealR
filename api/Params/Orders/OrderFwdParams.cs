namespace api.Params.Orders
{
    public class OrderFwdParams: PaginationParams
    {
        public int OrderId { get; set; }
        public DateTime? FwdDateFrom { get; set; }
        public DateTime FwdDateUpto { get; set; } 
        public int CustomerId { get; set; }
        public int ProfessionId { get; set; }
    }
}