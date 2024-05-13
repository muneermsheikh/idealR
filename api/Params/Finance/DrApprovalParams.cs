namespace api.Params.Finance
{
    public class DrApprovalParams: PaginationParams
    {
        public string Divn { get; set; }
        public string AccountType {get; set;}
        public string AccountName {get; set;}
        public long OpBalance {get; set;}
    }
}