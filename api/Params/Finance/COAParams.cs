namespace api.Params.Finance
{
    public class COAParams: PaginationParams
    {
        public int Id { get; set; }
        public string Divn { get; set; }
        public string AccountType {get; set;}
        public string AccountName {get; set;}
        public long OpBalance {get; set;}
    }
}