using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.DTOs.Order;
using api.Entities.Admin.Order;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Admin
{
    public class ContractReviewRepository: IContractReviewRepository
    {
          private readonly DataContext _context;
          private readonly IMapper _mapper;
          public ContractReviewRepository(DataContext context, IMapper mapper)
          {
               _mapper = mapper;
               _context = context;
          }

          public async Task<bool> EditContractReview(ContractReview model)
          {
               // thanks to @slauma of stackoverflow
               var existingObj = await _context.ContractReviews
               .Include(x => x.ContractReviewItems).ThenInclude(x => x.ContractReviewItemQs)
               .Where(p => p.Id == model.Id)
               .AsNoTracking()
               .SingleOrDefaultAsync() ?? throw new Exception("The Contract Review model does not exist in the database");

               if (existingObj.ContractReviewItems == null) throw new Exception("The Contract Review Items collection does not exist in the database");
               
               if (existingObj.ContractReviewItems.Any(x => x.ContractReviewItemQs == null)) 
                    throw new Exception("Review Parameters in one of the items do not exist");

               _context.Entry(existingObj).CurrentValues.SetValues(model);   //saves only the parent, not children

               //Delete children that exist in existing record, but not in the new model order
               foreach (var existingItem in existingObj.ContractReviewItems.ToList())
               {
                    if (!model.ContractReviewItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.ContractReviewItems.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               //children that are not deleted, are either updated or new ones to be added
               foreach (var itemModel in model.ContractReviewItems)
               {
                    var editedItem = await EditContractReviewItem(itemModel, true);
                    var existingItem = existingObj.ContractReviewItems.Where(x => x.Id == editedItem.Id).FirstOrDefault();
                    if(existingItem != null) {
                         _context.Entry(existingItem).CurrentValues.SetValues(editedItem);
                    } else {  //existingItem is a new object, with Id ==0
                         existingObj.ContractReviewItems.Add(existingItem);
                         _context.Entry(existingItem).State = EntityState.Added;
                    }
               }
               
               _context.Entry(existingObj).State = EntityState.Modified;        //this is doubling the contractreviewitems
               
               var recordsAffected=0;
               try {
                    recordsAffected = await _context.SaveChangesAsync();
               } catch (Exception ex) {
                    throw new Exception(ex.Message, ex);
               }

               if(recordsAffected > 0) {
                    await UpdateOrderReviewStatusNOSAVE(existingObj.OrderId, 0);    //2nd argument of OrderItemId not needed when first argument is available
               }

               return await _context.SaveChangesAsync() > 0;
               
          }

          //2nd parameter calledByThis - means this function is called within this module
          //, in which case the edited object is returned to calling program without SAVE
          //if it is called from outside this program, then this function saves the object
          //to the DB before returing
          public async Task<ContractReviewItem> EditContractReviewItem(ContractReviewItem model, bool calledByThis)
          {
               // thanks to @slauma of stackoverflow
               var existingObj = await _context.ContractReviewItems
                    .Where(p => p.Id == model.Id)
                    .Include(x => x.ContractReviewItemQs)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();

               if (existingObj == null || existingObj.ContractReviewItemQs == null) return null;

               _context.Entry(existingObj).CurrentValues.SetValues(model);   //saves only the parent, not children

               //Delete children that exist in existing record, but not in the new model order
               foreach (var existingItem in existingObj.ContractReviewItemQs.ToList())
               {
                    if (!model.ContractReviewItemQs.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.ContractReviewItemQs.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               //children that are not deleted, are either updated or new ones to be added
               foreach (var itemModelQ in model.ContractReviewItemQs)
               {
                    //work on the contractReviewItem
                    var existingQ = existingObj.ContractReviewItemQs.Where(c => c.Id == itemModelQ.Id && c.Id != default(int)).SingleOrDefault();
                    if (existingQ != null)       // record exists, update it
                    {
                         _context.Entry(existingQ).CurrentValues.SetValues(itemModelQ);
                         _context.Entry(existingQ).State = EntityState.Modified;
                    }
                    else            //record does not exist, insert a new record
                    {
                         var newItem = new ContractReviewItemQ {
                              OrderItemId=itemModelQ.OrderItemId, 
                              ContractReviewItemId= itemModelQ.ContractReviewItemId, 
                              SrNo = itemModelQ.SrNo, ReviewParameter = itemModelQ.ReviewParameter,
                              Response = itemModelQ.Response, ResponseText = itemModelQ.ResponseText, 
                              IsResponseBoolean=itemModelQ.IsResponseBoolean, 
                              IsMandatoryTrue=itemModelQ.IsMandatoryTrue, 
                              Remarks=itemModelQ.Remarks
                         };
                         existingObj.ContractReviewItemQs.Add(newItem);
                         _context.Entry(newItem).State = EntityState.Added;
                    }
               }

               _context.Entry(existingObj).State = EntityState.Modified;

               //update orderitem.reviewitemstatusId     
               existingObj.ReviewItemStatus = model.ReviewItemStatus;
               
               
               if(calledByThis) {
                    return existingObj;
               } else {
                    _context.Entry(existingObj).State = EntityState.Modified;
                    await UpdateOrderReviewStatusNOSAVE(0, model.OrderItemId);
                    return await _context.SaveChangesAsync() > 0 ? existingObj : null;
               }
               
          }

          public async Task<ContractReview> GetContractReviewFromOrderId(int orderId)
          {
               var crvw = await _context.ContractReviews.Where(x => x.OrderId == orderId)
                    .Include(x => x.ContractReviewItems)
                    .ThenInclude(x => x.ContractReviewItemQs)
                    .FirstOrDefaultAsync();
               return crvw;
          }

          public async Task<bool> DeleteContractReview(int orderid)
          {
               var contractReview = await _context.ContractReviews.Where(x => x.OrderId == orderid).FirstOrDefaultAsync() ?? throw new Exception("the object to delete does not exist");
               _context.ContractReviews.Remove(contractReview);
               _context.Entry(contractReview).State = EntityState.Deleted;      //configured as Delete.Cascade - all ConntractReviewItems and contractReviewItemQs will also be deleted
               try {
                    await _context.SaveChangesAsync();
               } catch (Exception ex) {
                    throw new Exception(ex.Message, ex);
               }
               return true;
          }

          public async Task<bool> DeleteContractReviewItem(int orderitemid)
          {
               var item = await _context.ContractReviewItems.Where(x => x.OrderItemId == orderitemid).FirstOrDefaultAsync() ?? throw new Exception("the object to delete does not exist");
               _context.ContractReviewItems.Remove(item);
               _context.Entry(item).State = EntityState.Deleted;           //configured as delete.cascade
               try{
                    await _context.SaveChangesAsync();
               } catch (Exception ex) {
                    throw new Exception (ex.Message, ex);
               }
               return true;
          }

          public async Task<bool> DeleteReviewQ(int id)
          {
               var reviewItem = await _context.ContractReviewItemQs.FindAsync(id);
               if (reviewItem == null) throw new Exception("the Review Question to delete does not exist");
               try {
                    await _context.SaveChangesAsync();
               } catch (Exception ex) {
                    throw new Exception(ex.Message, ex);
               }
               return true;
          }

          //returns the object, without inserting in DB
          public async Task<ContractReview> GetOrGenerateContractReviewNOSAVE(int orderId, string Username)
          {
               var contractreview = await _context.ContractReviews
                    .Include(x => x.ContractReviewItems)
                    .ThenInclude(x => x.ContractReviewItemQs)
                    .Where(x => x.OrderId == orderId)
                    .FirstOrDefaultAsync();
               
               if(contractreview != null) return contractreview;

               //now, generate a new contract review 
               var order = await _context.Orders.Where(x => x.Id == orderId)
                    .Include(x => x.OrderItems).ThenInclude(x => x.Remuneration)
                    .FirstOrDefaultAsync();
               
               var contractReviewItems = new List<ContractReviewItem>();
               var reviewQs = await _context.ContractReviewItemStddQs.OrderBy(x => x.SrNo).ToListAsync();
               var itemQs = (ICollection<ContractReviewItemQ>)_mapper.Map<ContractReviewItemQ>(reviewQs);

               var contractReview = new ContractReview
               {
                    OrderNo = order.OrderNo,
                    OrderId = orderId,
                    OrderDate = order.OrderDate,
                    CustomerId = order.CustomerId,
                    CustomerName = await _context.CustomerNameFromId(order.CustomerId),
                    ReviewedByName = Username,
                    ReviewedOn = System.DateTime.Now                   
               };

               foreach (var item in order.OrderItems)
               {
                    var newContractReviewItem = new ContractReviewItem
                    {
                         OrderItemId = item.Id,
                         OrderId = orderId,
                         Quantity = item.Quantity,
                         ProfessionName = await _context.GetProfessionNameFromId(item.ProfessionId),
                         Ecnr = item.Ecnr,
                         SourceFrom = item.SourceFrom,
                         ContractReviewItemQs = itemQs,
                         RequireAssess = "false"
                    };
                    contractReview.ContractReviewItems.Add(newContractReviewItem);
               }

               return contractReview;
          }

          public  ICollection<int> ConvertCSVToAray(string csv) {
               bool isParsingOk = true;
               int[] results = Array.ConvertAll<string,int>(csv.Split(','), 
               new Converter<string,int>(
               delegate(string num)
               {
                    int r;
                    isParsingOk &= int.TryParse(num, out r);
                    return r;
               }));
               return results;

               }
          public async Task<PagedList<ContractReviewDto>> GetContractReviews(ContractReviewParams cParams)
          {
               if (!string.IsNullOrEmpty(cParams.OrderIds)) {
                    cParams.OrderIdInts = ConvertCSVToAray(cParams.OrderIds);
               }

               var query = _context.ContractReviews
                    //.Where(x => cParams.OrderIdInts.Contains(x.OrderId))
                    .OrderBy(x => x.OrderNo)
                    .AsQueryable();
               
               var paged = await PagedList<ContractReviewDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<ContractReviewDto>(_mapper.ConfigurationProvider),
                    cParams.PageNumber, cParams.PageSize);
               
               return paged;

          }

          public async Task<ContractReview> AddContractReview(ContractReview contractReview)
          {
               _context.ContractReviews.Add(contractReview);
               //*TODO* data verification

               try {
                    await _context.SaveChangesAsync();
               } catch (Exception ex) {
                    throw new Exception(ex.Message, ex);
               }

               return contractReview;
          }
               
          public async Task<ICollection<ContractReviewItemStddQ>> GetReviewStddQs()
               {
                    var data = await _context.ContractReviewItemStddQs.OrderBy(x => x.SrNo).ToListAsync();
                    return data;
               }

          //based on status of ALL orderItem.ReviewItemStatusId, update OrderReviewStatus field
          private async Task<bool> UpdateOrderReviewStatusNOSAVE(int orderId, int orderitemid)
          {
               if(orderId == 0) {
                    orderId = await _context.OrderItems.Where(x => x.Id == orderitemid)
                         .Select(x => x.OrderId).FirstOrDefaultAsync();
               }

               if(orderId == 0) return false;
               var order = await _context.Orders.Include(x => x.OrderItems)
                    .Where(x => x.Id == orderId)
                    .FirstOrDefaultAsync();

               //if any item is not reviewed, return false
               var reviewitems = await _context.ContractReviewItems
                    .Where(x => x.OrderId == orderId)
                    .Include(x => x.ContractReviewItemQs.Where(x => x.ReviewParameter=="Service Charges in INR"))
                    .ToListAsync();
               
               if(reviewitems == null || reviewitems.Count == 0) {
                    if(order.Status != "Awaiting Review") {
                         order.Status="Awaiting Review";
                         order.ContractReviewStatus = "NotReviewed";
                         _context.Entry(order).State = EntityState.Modified;
                    }
                    return false;
               }

               order.ContractReviewStatus = order.OrderItems.Any(x => x.ReviewItemStatus.ToLower() == "regretted")
                    ? order.OrderItems.Any(x => x.ReviewItemStatus.ToLower()=="accepted") 
                         ? "Accepted With Regrets"
                         :  "Regretted"
                    : "Accepted";
               
               _context.Entry(order).State= EntityState.Modified;
     
               return true;
          }

          public async Task<bool> UpdateOrderReviewStatusWITHSAVE(int orderId, int orderitemid)
          {
               if(await UpdateOrderReviewStatusNOSAVE(orderId, orderitemid)) {
                    return await _context.SaveChangesAsync() > 0;
               } else {
                    return false;
               }
          }
          public async Task<ContractReviewItemDto> GetOrGenerateContractReviewItem(int orderItemId, string username)
          {
               var orderId=0;

               var existing = await _context.ContractReviewItems
                    .Where(x => x.OrderItemId == orderItemId)
                    .Include(x => x.ContractReviewItemQs)
                    .Select(x => new ContractReviewItemDto {
                         ProfessionName = x.ProfessionName, Charges=x.Charges,
                         ContractReviewId=x.ContractReviewId, ContractReviewItemQs = x.ContractReviewItemQs,
                         Ecnr = x.Ecnr, Id=x.Id, Quantity=x.Quantity,
                         OrderItemId=orderItemId, RequireAssess=x.RequireAssess, 
                         ReviewItemStatus=x.ReviewItemStatus,
                         SourceFrom=x.SourceFrom, HrExecUsername=x.HrExecUsername

                    })
                    .FirstOrDefaultAsync();
               
               if(existing != null) {
                    var items = await (from item in _context.OrderItems where item.Id == orderItemId
                         join order in _context.Orders on item.OrderId equals order.Id 
                         select new {
                              SrNo = item.SrNo, OrderId = item.OrderId, OrderNo=order.OrderNo,
                              OrderDate = order.OrderDate, OrderItemId = orderItemId,
                              CustomerName = order.Customer.CustomerName, CompleteBefore = item.CompleteBefore
                         }).FirstOrDefaultAsync();
                    
                    if(items != null) {
                         existing.SrNo = items.SrNo;
                         existing.OrderId = items.OrderId;
                         existing.OrderDate = items.OrderDate;
                         existing.OrderItemId = orderItemId;
                         existing.CustomerName = items.CustomerName;
                         existing.CompleteBefore = items.CompleteBefore;
                    }
                    orderId=items.OrderId;
               } else {
                    var qs = await _context.ContractReviewItemQs.Where(x => x.OrderItemId == orderItemId)
                         .OrderBy(x => x.SrNo).ToListAsync();

                    existing = await (from item in _context.OrderItems where item.Id == orderItemId 
                         join order in _context.Orders on item.OrderId equals order.Id
                         join cat in _context.Professions on item.ProfessionId equals cat.Id
                         //join reviewitem in _context.ContractReviewItems on item.Id equals reviewitem.OrderItemId 
                              //into grpReviewitem from rvwitem in grpReviewitem.DefaultIfEmpty()
                         select new  ContractReviewItemDto {
                              ProfessionName = cat.ProfessionName, Charges=0, SrNo = item.SrNo,
                              ContractReviewId= 0, ContractReviewItemQs = qs,
                              CustomerName=order.Customer.CustomerName, Ecnr = false, Id=0, OrderDate=order.OrderDate,
                              OrderId = order.Id, OrderNo = order.OrderNo, Quantity=item.Quantity,
                              OrderItemId=orderItemId, RequireAssess="No", ReviewItemStatus="Not Reviewed",
                              SourceFrom="India", HrExecUsername="", CompleteBefore = item.CompleteBefore
                         }).FirstOrDefaultAsync();
                         
                    orderId = existing.OrderId;
               }

               await UpdateOrderReviewStatusNOSAVE(orderId,0);    //only one of the two arguments - orderId or OrderItemId - need be provided
              
               return existing;
          }

          public async Task<ICollection<ContractReviewItem>> GetContractReviewItems(int orderid)
          {
               var query = await (from rvw in _context.ContractReviews where rvw.OrderId==orderid
                    join rvwItem in _context.ContractReviewItems on rvw.Id equals rvwItem.ContractReviewId
                    select rvwItem).ToListAsync();
               
               return query;
          }

        public async Task<ContractReviewItem> AddContractReviewItem(int orderitemid)
        {
               var contractreviewitem = await _context.ContractReviewItems.Include(x => x.ContractReviewItemQs)
                    .Where(x => x.OrderItemId == orderitemid).FirstOrDefaultAsync();

               if (contractreviewitem != null) return contractreviewitem;
            
               var orderitem = await _context.OrderItems.FindAsync(orderitemid);
               if(orderitem == null) return null;

               var contractreview = await _context.ContractReviews.Where(x => x.OrderId == orderitem.OrderId).FirstOrDefaultAsync(); 
               if(contractreview == null) return null;

               contractreviewitem = new ContractReviewItem{
                    ContractReviewId = contractreview.Id, OrderItemId=orderitemid, OrderId = orderitem.OrderId,
                    ProfessionName=orderitem.Profession.ProfessionName, Quantity=orderitem.Quantity,
                    RequireAssess="f", ReviewItemStatus="Not Reviewed", SourceFrom="India"
               };

               _context.Entry(contractreviewitem).State = EntityState.Added;

               return await _context.SaveChangesAsync() > 0 ? contractreviewitem : null;
            
        }

        public async Task<bool> EditContractReviewItemTODELETE(ContractReviewItem model)
        {
             // thanks to @slauma of stackoverflow
               var existingObj = await _context.ContractReviewItems
               .Include(x => x.ContractReviewItemQs)
               .Where(p => p.Id == model.Id)
               .AsNoTracking()
               .SingleOrDefaultAsync();

               if (existingObj == null) throw new Exception("The Contract Review Item model does not exist");
               if (existingObj.ContractReviewItemQs == null) throw new Exception("The Contract Review Items collection does not exist in the database");
        
               _context.Entry(existingObj).CurrentValues.SetValues(model);   //saves only the parent, not children

               //Delete children that exist in existing record, but not in the new model order
               foreach (var existingItem in existingObj.ContractReviewItemQs.ToList())
               {
                    if (!model.ContractReviewItemQs.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.ContractReviewItemQs.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               //children that are not deleted, are either updated or new ones to be added
               foreach(var newItem in model.ContractReviewItemQs)
               {
                    var existingItem = existingObj.ContractReviewItemQs
                         .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                    
                    if(existingItem != null)    //update navigation record
                    {
                         _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                         _context.Entry(existingItem).State = EntityState.Modified;
                    } else {    //insert new navigation record
                         var itemToInsert = new ContractReviewItemQ
                         {
                              OrderItemId=newItem.OrderItemId, 
                              ContractReviewItemId= newItem.ContractReviewItemId, 
                              SrNo = newItem.SrNo, ReviewParameter = newItem.ReviewParameter,
                              Response = newItem.Response, ResponseText = newItem.ResponseText, 
                              IsResponseBoolean=newItem.IsResponseBoolean, 
                              IsMandatoryTrue=newItem.IsMandatoryTrue, 
                              Remarks=newItem.Remarks
                         };

                         existingObj.ContractReviewItemQs.Add(itemToInsert);
                         _context.Entry(itemToInsert).State = EntityState.Added;
                    }
               }

               _context.Entry(existingObj).State = EntityState.Modified;

               try {
                    await _context.SaveChangesAsync();
               } catch (Exception ex) {
                    throw new Exception(ex.Message, ex);
               }

               return true;
               
        }

          public async Task<ICollection<ContractReviewItem>> GetContractReviewItemsFromOrderId(int orderId)
          {
               var qs = await _context.ContractReviewItemQs.OrderBy(x => x.SrNo).ToListAsync();

               var reviewitems = await (from orderitem in _context.OrderItems where orderitem.OrderId == orderId
                    join cat in _context.Professions on orderitem.ProfessionId equals cat.Id
                    select new ContractReviewItem {
                         Charges=0, ContractReviewId=0, Ecnr = orderitem.Ecnr, HrExecUsername="", OrderId=orderitem.OrderId,
                         OrderItemId=orderitem.Id, ProfessionName=cat.ProfessionName, Quantity=orderitem.Quantity,
                         RequireAssess="false", ReviewItemStatus="Not Reviewed", SourceFrom="", Id=0,
                          ContractReviewItemQs=qs
                    }).ToListAsync();
               
               return reviewitems;
          }
    }
}