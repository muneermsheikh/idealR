using System.ComponentModel.DataAnnotations;

namespace api.Entities.Master
{
    public class ChecklistHRData: BaseEntity
    {
        public ChecklistHRData()
          {
          }

          public ChecklistHRData(int srno, string parameter)
          {
               SrNo = srno;
               Parameter = parameter;
          }

        public int SrNo {get; set;}
        public string Parameter { get; set; }
        public bool IsMandatory {get; set;}=true;
    }
}