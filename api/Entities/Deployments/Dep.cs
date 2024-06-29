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
        public int CvRefId { get; set; }
        public int OrderItemId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CityOfWorking { get; set; }
        public DateTime SelectedOn { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime CurrentStatusDate { get; set; }
        public bool Ecnr { get; set; }=false;
        public ICollection<DepItem> DepItems { get; set; }
        public CVRef CVRef { get; set; }

    }


}