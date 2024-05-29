namespace api.Entities.Master
{
    public class Help: BaseEntity
    {
        public string Topic { get; set; }
        public ICollection<HelpItem> HelpItems { get; set; }
        
    }
}