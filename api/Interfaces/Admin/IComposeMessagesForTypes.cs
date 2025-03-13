using api.DTOs.Admin;
using api.Entities.Admin.Order;
using api.Entities.HR;

namespace api.Interfaces.Admin
{
    public interface IComposeMessagesForTypes
    {
        public string GetSelectionDetails(string CandidateName, int ApplicationNo, string CustomerName, 
            string CategoryName, Employment employmt);
        Task<string> ComposeOrderItems(int orderNo, ICollection<OrderItem> orderItems, bool hasException);
        string GetSelectionDetailsBySMS(SelectionDecision selection);
        Task<string> TableOfOrderItemsContractReviewedAndApproved(ICollection<int> itemIds);
        Task<OrderItemReviewStatusDto> CumulativeCountForwardedSoFar(int orderitemId);
        Task<string> AssessmentGrade(int candidateId, int orderitemId);
        //Task<string> TableOfCVsSubmittedByHRExecutives(ICollection<CVsSubmittedDto> cvsSubmitted);
        //Task<string> TableOfCVsSubmittedByHRSup(ICollection<CVsSubmittedDto> cvsSubmitted);
        Task<string> TableOfRelevantOpenings(List<int> Ids);
    }
}