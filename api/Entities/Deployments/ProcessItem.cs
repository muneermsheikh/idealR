namespace api.Entities.Deployments
{
    public class ProcessItem: BaseEntity
    {
        public int ProcessId { get; set; }
        public DateOnly TransactionDate { get; set; }   
        public int Sequence { get; set; }
        public int NextSequence { get; set; }
        public DateOnly NextSequenceDate { get; set; }
    }
}