namespace api.DTOs.Finance
{
    public class StatementOfAccountDto
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
		public DateTime FromDate {get; set;}
		public DateTime UptoDate {get; set;}
		public long OpBalance {get; set;}
		public long ClBalance {get; set;}	//balance for the period
        public ICollection<StatementOfAccountItemDto> StatementOfAccountItems { get; set; }
    }
}