namespace api.Params.Admin
{
    public class SelDecisionParams: PaginationParams
    {
        public int CVRefId { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId {get; set;}
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public DateTime SelectedOn { get; set; }
        public string RefStatus { get; set; }
    }
}