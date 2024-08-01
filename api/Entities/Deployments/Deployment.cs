namespace api.Entities.Deployments
{
    public class Deployment: BaseEntity
    {
        public int CVRefId { get; set; }
        public DateTime TransactionDate { get; set; }   
        public int Sequence { get; set; }
        public int NextSequence { get; set; }
        public DateTime NextSequenceDate { get; set; }
    }
}