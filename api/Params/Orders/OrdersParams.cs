namespace api.Params.Orders
{
    public class OrdersParams: PaginationParams
    {
        public int Id { get; set; }
        public int OrderNo { get; set; }
        public int CustomerId { get; set; }

    }
}