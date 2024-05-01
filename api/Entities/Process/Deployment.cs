using System.ComponentModel.DataAnnotations.Schema;
using api.Entities.HR;

namespace api.Entities.Process
{
    public class Deployment: BaseEntity
    {
        [ForeignKey("CVRefId")]
        public int CVRefId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int Sequence { get; set; }
        public int NextSequence { get; set; }
        public DateTime NextSequenceDate { get; set; }
        public CVRef CVRef {get; set;} 
    }
}