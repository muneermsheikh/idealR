namespace api.Params.Finance
{
    public class VoucherParams: PaginationParams
    {
        public string Divn {get; set;}
		public int VoucherNo {get; set;}
		public DateTime VoucherDated {get; set;}
		public int CoaId {get; set;}
		public string AccountName {get; set;}
		public DateTime DateFrom {get; set;}
		public DateTime DateUpto {get; set;}
		public long Amount { get; set; }
		
    }
}