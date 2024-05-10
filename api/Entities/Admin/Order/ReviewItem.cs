namespace api.Entities.Admin.Order
{
    public class ReviewItem
    {
        public int OrderItemId { get; set; }
        public int ContractReviewItemId { get; set; }
        public int SrNo { get; set; }
        public string ReviewParameter { get; set; }
        public bool Response { get; set; }
        public string ResponseText {get; set;}
        public bool IsResponseBoolean {get; set;}
        public bool IsMandatoryTrue { get; set; }=false;
        public string Remarks { get; set; }
    }
}