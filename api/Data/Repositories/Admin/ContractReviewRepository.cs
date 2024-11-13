using api.DTOs.Admin;
using api.DTOs.Order;
using api.Entities.Admin.Order;
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
        private readonly IOrderForwardRepository _orderFwdRepo;
          public ContractReviewRepository(DataContext context, IOrderForwardRepository orderFwdRepo, IMapper mapper)
          {
            _orderFwdRepo = orderFwdRepo;
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
                    await _orderFwdRepo.UpdateOrderExtnDueToDelete(orderid, "ContractReview");
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

               var reviewQs = await _context.ContractReviewItemStddQs.OrderBy(x => x.SrNo).ToListAsync();
               
               var contractRvwItemQs = new List<ContractReviewItemQ>();
               foreach(var rvwQ in reviewQs) {
                    contractRvwItemQs.Add(new ContractReviewItemQ{
                         SrNo=rvwQ.SrNo, ResponseText=rvwQ.ResponseText,
                         ReviewParameter=rvwQ.ReviewParameter,Response=false,
                         IsResponseBoolean=false, IsMandatoryTrue=rvwQ.IsMandatoryTrue,
                         Remarks=""
                    });
               }

               var contractReviewItems = await (from item in _context.OrderItems where item.OrderId==orderId
                    join prof in _context.Professions on item.ProfessionId equals prof.Id
                    orderby item.SrNo
                    select new ContractReviewItem {

                         OrderItemId = item.Id,
                         OrderId = orderId,
                         Quantity = item.Quantity,
                         ProfessionName = prof.ProfessionName,
                         Ecnr = item.Ecnr,
                         SourceFrom = item.SourceFrom,
                         ContractReviewItemQs = contractRvwItemQs,
                         RequireAssess = "false"
                    }).ToListAsync();
               
               
               var contractReview = await (from order in _context.Orders where order.Id==orderId
                    select new ContractReview {
                         OrderNo = order.OrderNo,
                         OrderId = orderId,
                         OrderDate = order.OrderDate,
                         CustomerId = order.CustomerId,
                         CustomerName = order.Customer.CustomerName,
                         ReviewedByName = Username,
                         ReviewedOn = System.DateTime.Now,
                         ContractReviewItems=contractReviewItems
                    }).FirstOrDefaultAsync();
 
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

                    //update orderextns
                    await _orderFwdRepo.UpdateOrderExtn(contractReview.OrderId, "ContractReview","");

                    var order = await _context.Orders.FindAsync(contractReview.OrderId);
               
                    switch (contractReview.ReviewStatus.ToLower()) {
                         case "accepted":
                              order.ContractReviewStatus="Accepted";
                              order.ContractReviewId=contractReview.Id;
                              
                              break;
                         case "regretted":
                              order.ContractReviewStatus="Regretted";
                              order.ContractReviewId = contractReview.Id;

                              break;
                         case "accepted with regrets":
                              order.ContractReviewStatus="Accepted With Regrets";
                              order.ContractReviewId=contractReview.Id;

                              break;
                         default:
                              break;
                    }

               } catch {
                    return null;
               }

               try {
                    await _context.SaveChangesAsync();
               } catch {
                    return null;
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
               var orderdata = await (from item in _context.OrderItems where item.Id == orderItemId
                    join order in _context.Orders on item.OrderId equals order.Id
                    select new {OrderNo = order.OrderNo, OrderDate = order.OrderDate, OrderId=order.Id,
                    CustomerId = order.CustomerId, CustomerName = order.Customer.CustomerName, 
                    CompleteBefore = item.CompleteBefore}
                    ).FirstOrDefaultAsync();

               var existing = await _context.ContractReviewItems
                    .Where(x => x.OrderItemId==orderItemId)
                    .Include(x => x.ContractReviewItemQs)
                    .FirstOrDefaultAsync()
                    ?? await (from item in _context.OrderItems where item.Id==orderItemId
                         join order in _context.Orders on item.OrderId equals order.Id
                         select new ContractReviewItem {
                              OrderId = order.Id, ProfessionName = item.Profession.ProfessionName, 
                              Quantity = item.Quantity, Ecnr= item.Ecnr, SourceFrom="India", OrderItemId=orderItemId
                         }).FirstOrDefaultAsync();
               if(existing.ContractReviewItemQs==null || existing.ContractReviewItemQs.Count ==0) {
                    var stddQs = await _context.ContractReviewItemStddQs.OrderBy(x => x.SrNo).ToListAsync();
                    var listQs = new List<ContractReviewItemQ>();
                    foreach(var q in stddQs) {
                         var itemQ = new ContractReviewItemQ {
                              OrderItemId = orderItemId, IsMandatoryTrue = q.IsMandatoryTrue, 
                              Response = false, ReviewParameter= q.ReviewParameter, SrNo = q.SrNo,
                              ContractReviewItemId=existing.Id, ResponseText=q.ResponseText};
                         listQs.Add(itemQ);
                    }
                    existing.ContractReviewItemQs = listQs;
               }

               if(existing.ContractReviewId == 0) {
                    var Review = await _context.ContractReviews.Where(x => x.OrderId == orderdata.OrderId).FirstOrDefaultAsync();
                    if(Review == null) {
                         Review =new ContractReview{OrderId=orderdata.OrderId, CustomerId=orderdata.CustomerId,
                         CustomerName=orderdata.CustomerName, OrderDate = orderdata.OrderDate, 
                         OrderNo=orderdata.OrderNo, ContractReviewItems = new List<ContractReviewItem>{existing}};
                    _context.Entry(Review).State = EntityState.Added;
                    } else {
                         existing.ContractReviewId = Review.Id;
                    }
               } else {
                    if(existing.Id==0) {
                         _context.Entry(existing).State = EntityState.Added;
                     } else {
                         _context.Entry(existing).State = EntityState.Modified;
                     } 
               }
               
               try {
                    var ct = await _context.SaveChangesAsync();
                    var id = existing.Id;
                    var dto = _mapper.Map<ContractReviewItemDto>(existing);
                    dto.OrderNo = orderdata.OrderNo;
                    dto.OrderDate = orderdata.OrderDate;
                    dto.CustomerName = orderdata.CustomerName;
                    dto.CompleteBefore = orderdata.CompleteBefore;
                    dto.Id = id;

                    return dto;
               } catch {
                    return null;
               }
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

          public async Task<ContractReviewItemDto> InsertContractRvwItemFromOrderItemAndContractRvwId(int orderitemid)
          {
               var contractReviewItemId=0;
               var contractReviewItem = new ContractReviewItem();

               var reviewitem = await _context.ContractReviewItems.Where(x => x.OrderItemId==orderitemid)
                    .Include(x => x.ContractReviewItemQs)
                    .FirstOrDefaultAsync();
               if(reviewitem != null) {
                    if(reviewitem.ContractReviewItemQs.Count == 0) {
                         reviewitem.ContractReviewItemQs = await CreateNewContractReviewItemQs(orderitemid, reviewitem.Id);
                         _context.Entry(reviewitem).State = EntityState.Modified;
                         await _context.SaveChangesAsync();
                    }
                    return _mapper.Map<ContractReviewItemDto>(reviewitem);
               }

               if(reviewitem != null) contractReviewItemId=reviewitem.Id;

               /*var stddQs = await _context.ContractReviewItemStddQs.OrderBy(x => x.SrNo).ToListAsync();
               var reviewItemQs = new List<ContractReviewItemQ>();
               var srno=1;
               stddQs.ForEach(x => {
                    var itemQ = new ContractReviewItemQ{OrderItemId=orderitemid,
                         ContractReviewItemId = 0, SrNo = srno++, 
                         ReviewParameter = x.ReviewParameter, Response=false, ResponseText="",
                         IsResponseBoolean=false, IsMandatoryTrue=x.IsMandatoryTrue, Remarks=""};
                    reviewItemQs.Add(itemQ);
               });
               */
               var qs=await CreateNewContractReviewItemQs(orderitemid, 0);
               var qry = await(from orderitem in _context.OrderItems where  orderitem.Id==orderitemid
                    join cat in _context.Professions on orderitem.ProfessionId equals cat.Id
                    join order in _context.Orders on orderitem.OrderId equals order.Id
                    select new ContractReviewItem{
                         ContractReviewId=0, OrderItemId=orderitemid, OrderId=orderitem.OrderId,
                         ProfessionName = cat.ProfessionName, Quantity=orderitem.Quantity, Ecnr=orderitem.Ecnr,
                         SourceFrom="India", RequireAssess="t", Charges=0,HrExecUsername="",
                         ReviewItemStatus="Not Reviewed", ContractReviewItemQs=qs
                         //, OrderNo = order.OrderNo, OrderDate=order.OrderDate, CustomerName=order.Customer.CustomerName
                         }
               ).FirstOrDefaultAsync();

               _context.Entry(qry).State=EntityState.Added;

               return await _context.SaveChangesAsync() > 0 ? _mapper.Map<ContractReviewItemDto>(qry) : null;
          }

          private async Task<ICollection<ContractReviewItemQ>> CreateNewContractReviewItemQs(int orderitemid, int contractReviewItemId)
          {
               var stddQs = await _context.ContractReviewItemStddQs.OrderBy(x => x.SrNo).ToListAsync();
               var reviewItemQs = new List<ContractReviewItemQ>();
               var srno=1;
               stddQs.ForEach(x => {
                    var itemQ = new ContractReviewItemQ{OrderItemId=orderitemid,
                         ContractReviewItemId = contractReviewItemId, SrNo = srno++, 
                         ReviewParameter = x.ReviewParameter, Response=false, ResponseText="",
                         IsResponseBoolean=false, IsMandatoryTrue=x.IsMandatoryTrue, Remarks=""};
                    reviewItemQs.Add(itemQ);
               });
               //await _context.SaveChangesAsync();
               return reviewItemQs;
          }
 
        public async Task<ContractReviewItem> SaveNewContractReviewItem(ContractReviewItem model)
        {
               var existingObj = await _context.ContractReviewItems
                    .Where(p => p.Id == model.Id)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();

               if (existingObj != null ) return null;       //record already exists

               //check if contractReview exists
               var data = await (from item in _context.OrderItems where item.Id == model.OrderItemId
                    join order in _context.Orders on item.OrderId equals order.Id 
                    select new {OrderId=order.Id, OrderNo=order.OrderNo, OrderDate=order.OrderDate,
                    CustomerId=order.CustomerId, CustomerName=order.Customer.CustomerName,
                    ProfessionId=item.ProfessionId, ProfessionName=item.Profession.ProfessionName,
                    Ecnr=item.Ecnr,Quantity=item.Quantity}
               ).FirstOrDefaultAsync();

               var reviewId = await _context.ContractReviews.Where(x => x.OrderId == data.OrderId)
                    .Select(x => x.Id).FirstOrDefaultAsync();
               if(reviewId==0) {
                    var review = new ContractReview{OrderId=data.OrderId, OrderNo=data.OrderNo,
                         OrderDate=data.OrderDate, CustomerId = data.CustomerId, 
                         CustomerName=data.CustomerName, ReviewedOn=DateTime.UtcNow};
                    _context.Entry(review).State = EntityState.Added;
                    await _context.SaveChangesAsync();
                    reviewId=review.Id;
               }

               var reviewItem = new ContractReviewItem{OrderId=data.OrderId, ContractReviewId=reviewId,
                    Ecnr = data.Ecnr, OrderItemId = model.OrderItemId, ProfessionName = data.ProfessionName,
                    Quantity = data.Quantity, SourceFrom = "India"};

               var Qs = new List<ContractReviewItemQ>();
               foreach(var q in model.ContractReviewItemQs) {
                    var itemQ = new ContractReviewItemQ{OrderItemId = q.OrderItemId,
                         IsMandatoryTrue = q.IsMandatoryTrue, IsResponseBoolean = q.IsResponseBoolean,
                         Remarks = q.Remarks, Response = q.Response, ResponseText = q.ResponseText,
                         ReviewParameter = q.ReviewParameter, SrNo = q.SrNo};
                    Qs.Add(itemQ);
               }
               
               reviewItem.ContractReviewItemQs=Qs;
               _context.Entry(reviewItem).State = EntityState.Added;

               try {
                    await _context.SaveChangesAsync();
                    return reviewItem;
               } catch {
                    return null;
               }
        }
    }

}
