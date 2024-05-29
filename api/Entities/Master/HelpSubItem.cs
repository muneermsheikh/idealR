namespace api.Entities.Master
{
    public class HelpSubItem: BaseEntity
    {
        public int helpId { get; set; }
        public int sequence { get; set; }
        public string SubText { get; set; }
    }
}