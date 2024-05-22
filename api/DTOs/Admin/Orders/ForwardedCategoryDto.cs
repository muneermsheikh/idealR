namespace api.DTOs.Admin.Orders
{
    public class ForwardedCategoryDto
    {
        public int Id {get; set;}
        public int OrderNo {get; set;}
        public DateTime OrderDate { get; set; }
		public int OrderItemId {get; set;}
		public string CategoryRefAndName {get; set;}
		public int Charges {get; set;}
		public ICollection<ForwardedOfficialDto> ForwardedOfficials {get; set;}
    }
}