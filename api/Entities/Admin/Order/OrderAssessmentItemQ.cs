using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin.Order
{
    public class OrderAssessmentItemQ: BaseEntity
    {
        public int OrderAssessmentItemId { get; set; }
        public int QuestionNo { get; set; }
        public string Subject { get; set; }
        public string Question { get; set; }
        [Range(1,50)]
        public int MaxPoints { get; set; }
        public bool IsMandatory { get; set; }=true;
    }
}