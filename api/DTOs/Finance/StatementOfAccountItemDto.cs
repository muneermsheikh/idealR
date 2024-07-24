namespace api.DTOs.Finance
{
    public class StatementOfAccountItemDto
    {
        public int Id { get; set; }
        public int VoucherNo { get; set; }
        public DateOnly TransDate { get; set; }
        public int CoaId { get; set; }
        public string AccountName { get; set; }
        public long Dr {get; set;}
	    public long Cr {get; set;}
        public string Narration { get; set; }
    }
}