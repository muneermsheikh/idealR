namespace api.Entities.Admin.Order
{
    public class OrderItemAssessmentQ: BaseEntity
    {
        public int OrderItemId { get; set; }
        public int QuestionNo { get; set; }
        public string Subject { get; set; }
        public string Question { get; set; }
        public int MaxPoints { get; set; }
        public bool IsMandatory { get; set; }
    }
}