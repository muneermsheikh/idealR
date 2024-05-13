namespace api.Entities.Deployments
{
    public class DepItem: BaseEntity
    {
        public DepItem()
        {
        }


        public int DepId { get; set; }
        public DateOnly TransactionDate { get; set; }   
        public int Sequence { get; set; }
        public int NextSequence { get; set; }
        public DateOnly NextSequenceDate { get; set; }
    }
}