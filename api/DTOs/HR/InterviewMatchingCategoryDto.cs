

namespace api.DTOs.HR
{
    public class InterviewMatchingCategoryDto
    {
        public int Id { get; set; }
        public bool Checked { get; set; }
        public string CustomerName { get; set; }
        public string CategoryName { get; set; }
        public DateTime InterviewDateFrom { get; set; }
    }
}