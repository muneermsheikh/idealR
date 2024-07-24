namespace api.Params.Finance
{
    public class ParamsCOA: PaginationParams
    {
        public int Id { get; set; }
        public string AccountName {get; set;}
        public int COAId { get; set; }
        public string Divn { get; set; }
        public string AccountType {get; set;}
        public string DivisionToExclude { get; set; }
   
    }
}