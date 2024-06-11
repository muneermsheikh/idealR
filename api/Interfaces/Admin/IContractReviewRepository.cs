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
        Task<ContractReview> EditContractReview(ContractReview contractReview);
        Task<ContractReview> GetOrGenerateContractReview(int orderId, string Username);
        Task<ContractReview> GetContractReviewFromOrderId(int orderId);
        Task<ContractReviewItemDto> GetContractReviewItem(int orderItemId, string username);
        Task<ICollection<ContractReviewItem>> GetContractReviewItems (int orderid);
        Task<bool> EditContractReviewItem(ContractReviewItem contractReviewItem);
        
        Task<bool> DeleteContractReview(int orderid);
        Task<bool> DeleteContractReviewItem(int orderitemid);
        Task<bool> DeleteReviewQ(int id);

        Task<ICollection<ContractReviewItemStddQ>> GetReviewStddQs();
        Task<bool> UpdateOrderReviewStatus(int orderId);
    }
}