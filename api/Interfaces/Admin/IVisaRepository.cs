using api.DTOs.Admin;
using api.Entities.Admin;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface IVisaRepository
    {
         Task<bool> DeleteVisa(int VisaIId);
         Task<PagedList<VisaBriefDto>> GetVisaPagedList(VisaParams visaParams);
         Task<PagedList<VisaTransaction>> GetVisaTransactionPagedList(VisaParams vParams);
         Task<VisaHeader> GetVisaHeader(int VisaHeaderId);
         Task<VisaHeader> InsertVisa(VisaHeader visaHeader);
         Task<VisaItem> InsertVisaItem(VisaItem visaitem);
         Task<VisaHeader> EditVisa(VisaHeader visaHeader);
         Task<bool> DeleteVisaItem(int VisaItemId);
         Task<VisaTransaction> InsertVisaTransaction(VisaTransaction vTransaction);
         Task<VisaTransaction> EditVisaTransaction(VisaTransaction vTransaction);
         Task<bool> DeleteVisaTransaction(int visaTransactionId);
         Task<ICollection<OrderItemForVisaAssignmentDto>> GetOpenOrderItemsForCustomer(int customerId);
         Task<ICollection<VisaAssignment>> InsertVisaAssignments(ICollection<VisaAssignment> visaAssignments);
    }
}