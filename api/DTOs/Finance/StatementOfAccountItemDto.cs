namespace api.DTOs.Finance
{
    public class StatementOfAccountItemDto
    {
        public int VoucherNo { get; set; }
        public DateTime TransDate { get; set; }
        public int COAId { get; set; }
        public string AccountName { get; set; }
        public long Dr {get; set;}
	    public long Cr {get; set;}
        public string Narration { get; set; }
    }
}