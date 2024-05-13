using api.Entities.HR;

namespace api.Entities.Deployments
{
    public class Process: BaseEntity
    {
        public Process()
        {
        }


        public int CVRefId { get; set; }
        public DateOnly SelectedOn { get; set; }
        public string CurrentStatus { get; set; }
        public ICollection<ProcessItem> ProcessItems { get; set; }
        public CVRef CVRef { get; set; }

    }


}