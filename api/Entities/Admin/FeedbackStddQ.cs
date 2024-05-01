namespace api.Entities.Admin
{
    public class FeedbackStddQ: BaseEntity
    {
        public string FeedbackGroup { get; set; }
        public int FeedbackQNo { get; set; }
        public string FeedbackQuestion { get; set; }
        public bool IsMandatory { get; set; }
        public string TextForLevel1 { get; set; }
        public string TextForLevel2 { get; set; }
        public string TextForLevel3 { get; set; }
        public string TextForLevel4 { get; set; }
    }
}