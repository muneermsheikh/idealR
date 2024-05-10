namespace api.Params.Finance
{
    public class VoucherParams
    {
        public string Divn {get; set;}
		public string VoucherNo {get; set;}
		public DateOnly VoucherDated {get; set;}
		public int CoaId {get; set;}
		public string AccountName {get; set;}
		public DateOnly DateFrom {get; set;}
		public DateOnly DateUpto {get; set;}
		public long Amount { get; set; }
		public int EmployeeId { get; set; }
		public string Narration {get; set;}
    }
}