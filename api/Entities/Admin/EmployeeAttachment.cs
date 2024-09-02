namespace api.Entities.Admin
{
    public class EmployeeAttachment : BaseEntity
    {
        public int EmployeeId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }      
        public string FullPath { get; set; }

    }
}