namespace api.DTOs.Process
{
    public class NextDepDataDto
    {
        public string CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public string ErrorString { get; set; }
        public ICollection<Referral> Referrals{ get; set; }
    }

    public class Referral
    {
        public int DepId {get; set; }
        public string CategoryRef { get; set; }
        public string CustomerName { get; set; }
        public DateTime ReferredOn { get; set; }
        public int Sequence {get; set;}
        public string SequenceName { get; set; }
        public int Period { get; set; }
    }
}