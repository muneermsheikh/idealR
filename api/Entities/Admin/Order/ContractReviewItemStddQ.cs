namespace api.Entities.Admin.Order
{
    public class ContractReviewItemStddQ: BaseEntity
    {
        public int SrNo { get; set; }
        public string ReviewParameter { get; set; }
        public string ResponseText {get; set;}
        public string ButtonText { get; set; }
        public string Button2Text { get; set; }
        public bool TextInput {get; set;}=false;
        public bool IsMandatoryTrue { get; set; }=false;
    }
}