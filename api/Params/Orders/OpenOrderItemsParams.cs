namespace api.Params.Orders
{
    public class OpenOrderItemsParams: PaginationParams
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public List<int> ProfessionIds { get; set; }
        public List<int> OrderItemIds { get; set; }
    }
}