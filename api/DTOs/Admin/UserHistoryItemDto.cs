namespace api.DTOs.Admin
{
    public class CallRecordItemDto
    {
        public int Id { get; set; }
        public int CallRecordId { get; set; }
        public DateOnly DateOfContact { get; set; }
        public string Username { get; set; }
        public string GistOfDiscussions { get; set; }
        public string ContactResult { get; set; }
    }
}