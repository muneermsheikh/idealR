namespace api.Params.Admin
{
    public class CustomerParams: PaginationParams
    {
        public int CustomerId { get; set; }
        public string CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string customerCity { get; set; }
    }
}