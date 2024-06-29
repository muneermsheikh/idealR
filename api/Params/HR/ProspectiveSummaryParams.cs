namespace api.Params.HR
{
    public class ProspectiveSummaryParams: PaginationParams
    {
        public string CategoryRef {get; set;}
        public DateTime Dated { get; set; }
        public string Status {get; set;}
    }
}