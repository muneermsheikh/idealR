namespace api.Entities.Master
{
    public class HelpSubItem: BaseEntity
    {
        public int HelpItemId { get; set; }
        public int Sequence { get; set; }
        public string HelpText { get; set; }
    }
}