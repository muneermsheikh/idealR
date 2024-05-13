using System.ComponentModel.DataAnnotations.Schema;
using api.Entities.HR;

namespace api.Entities.Deployments
{
    public class Dep: BaseEntity
    {
        public Dep()
        {
        }

        //[ForeignKey("SelectionDecisionId")]
        //public int SelectionDecisionId { get; set; }
        public int CVRefId { get; set; }
        public int OrderItemId { get; set; }
        public int CustomerId { get; set; }
        public DateOnly SelectedOn { get; set; }
        public string CurrentStatus { get; set; }
        public ICollection<DepItem> DepItems { get; set; }
        public CVRef CVRef { get; set; }

    }


}