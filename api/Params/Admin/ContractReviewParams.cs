namespace api.Params.Admin
{
    public class ContractReviewParams: PaginationParams
    {
        public string OrderIds { get; set; }    //csv
        public ICollection<int> OrderIdInts { get; set; }
        public int OrderNo { get; set; }
        public int CustomerId { get; set; }
        public bool ReleasedForProduction { get; set; }
    }
}