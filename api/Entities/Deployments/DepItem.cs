namespace api.Entities.Deployments
{
    public class DepItem: BaseEntity
    {
        public DepItem()
        {
        }

        public int DepId { get; set; }
        public DateTime TransactionDate { get; set; }   
        public int Sequence { get; set; }
        public int NextSequence { get; set; }
        public DateTime NextSequenceDate { get; set; }
        public string FullPath { get; set; }
   
    }
}