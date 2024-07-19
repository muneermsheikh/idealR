using api.DTOs.Admin;
using api.DTOs.Order;
using api.Entities.Admin.Order;
using api.Entities.Master;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface IContractReviewRepository
    {
        Task<PagedList<ContractReviewDto>> GetContractReviews(ContractReviewParams cParams);
        Task<ContractReview> AddContractReview(ContractReview contractReview);
        Task<ContractReviewItem> AddContractReviewItem(int orderitemid);
        Task<bool> EditContractReview(ContractReview contractReview);
        Task<bool> EditContractReviewItemTODELETE(ContractReviewItem contractReviewItem);
        Task<ContractReviewItem> EditContractReviewItem(ContractReviewItem contractReviewItem, bool calledFromWithinThisModule);
        Task<ContractReview> GetOrGenerateContractReviewNOSAVE(int orderId, string Username);
        Task<ICollection<ContractReviewItem>> GetContractReviewItemsFromOrderId(int orderId);
        Task<ContractReview> GetContractReviewFromOrderId(int orderId);
        Task<ContractReviewItemDto> GetOrGenerateContractReviewItem(int orderItemId, string username);
        Task<ICollection<ContractReviewItem>> GetContractReviewItems (int orderid);
        
        
        Task<bool> DeleteContractReview(int orderid);
        Task<bool> DeleteContractReviewItem(int orderitemid);
        Task<bool> DeleteReviewQ(int id);

        Task<ICollection<ContractReviewItemStddQ>> GetReviewStddQs();
        Task<bool> UpdateOrderReviewStatusWITHSAVE(int orderId, int orderItemId);
    }
}