namespace api.Params.Finance
{
    public class VoucherParams: PaginationParams
    {
        public string Divn {get; set;}
		public int VoucherNo {get; set;}
		public DateOnly VoucherDated {get; set;}
		public int CoaId {get; set;}
		public string AccountName {get; set;}
		public DateOnly DateFrom {get; set;}
		public DateOnly DateUpto {get; set;}
		public long Amount { get; set; }
		
    }
}