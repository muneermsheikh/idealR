namespace api.Entities.HR
{
    public class ChecklistHRItem: BaseEntity
    {
        public int ChecklistHRId { get; set; }
        public int SrNo {get; set;}
        public string Parameter {get; set;}
        public bool Accepts {get; set;}=false;
        public string Response {get; set;}
        public string Exceptions {get; set;}
        public bool MandatoryTrue {get; set;}
    }
}