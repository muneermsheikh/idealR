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

          public async Task<ContractReview> EditContractReview(ContractReview model)
          {
               // thanks to @slauma of stackoverflow
               var existingObj = await _context.ContractReviews
               .Include(x => x.ContractReviewItems).ThenInclude(x => x.ContractReviewItemQs)
               .Where(p => p.Id == model.Id)
               .AsNoTracking()
               .SingleOrDefaultAsync();

               if (existingObj == null) throw new Exception("The Contract Review model does not exist in the database");
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
               var reviewStatus = existingObj.ContractReviewItems.Any(x => x.ReviewItemStatus == "NotReviewed") 
                    ? "NotReviewed"
                    : existingObj.ContractReviewItems.Any(x => x.ReviewItemStatus != "Accepted")
                         && existingObj.ContractReviewItems.Any(x => x.ReviewItemStatus == "Accepted")
                    ? "AcceptedWithSomeRegrets"
                    : "Accepted";
               existingObj.ReviewStatus = reviewStatus;

               _context.Entry(existingObj).State = EntityState.Modified;        //this is doubling the contractreviewitems

               try {
                    await _context.SaveChangesAsync();
               } catch (Exception ex) {
                    throw new Exception(ex.Message, ex);
               }

               return existingObj;
               
          }


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
          var contractReview = await _context.ContractReviews.Where(x => x.OrderId == orderid).FirstOrDefaultAsync();
          if (contractReview == null) throw new Exception("the object to delete does not exist");
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
          var item = await _context.ContractReviewItems.Where(x => x.OrderItemId == orderitemid).FirstOrDefaultAsync();
          if (item == null) throw new Exception("the object to delete does not exist");
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
     public async Task<ContractReview> GenerateContractReviewObject(int orderId, string Username)
     {
          //check if Remuneration for all the order items exist
          var itemIds = await _context.OrderItems.Where(x => x.OrderId == orderId).Select(x => x.Id).ToListAsync();
          
          if (itemIds.Count == 0) throw new Exception("Order Items not created");

          var orderitems = await _context.OrderItems.Where(x => itemIds.Contains(x.Id))
               .Include(x => x.Remuneration)
               .ToListAsync();
          if (orderitems == null || orderitems.Count == 0) throw new Exception("Remunerations need to be defined for all the items before the contract review");

          //check if the object exists
          var contractReview = await _context.ContractReviews.Where(x => x.OrderId == orderId).Include(x => x.ContractReviewItems).FirstOrDefaultAsync();
          if (contractReview != null) throw new System.Exception("Contract Review Object already exists");

          var order = await _context.Orders.Where(x => x.Id == orderId)
               .Include(x => x.OrderItems).FirstOrDefaultAsync();
          var contractReviewItems = new List<ContractReviewItem>();
          var reviewQs = await _context.ContractReviewItemStddQs.OrderBy(x => x.SrNo).ToListAsync();

          foreach (var item in order.OrderItems)
          {
               var requireAssess = contractReview.ContractReviewItems.Where(x => x.OrderItemId == item.Id).Select(x => x.RequireAssess).FirstOrDefault();
               var itemQs = new List<ContractReviewItemQ>();
               foreach (var data in reviewQs)
               {
                    itemQs.Add(new ContractReviewItemQ
                    {
                         SrNo = data.SrNo, OrderItemId = item.Id, ReviewParameter = data.ReviewParameter, IsMandatoryTrue = data.IsMandatoryTrue
                    });
               }
               contractReviewItems.Add(new ContractReviewItem
               {
                    OrderItemId = item.Id,
                    Quantity = item.Quantity,
                    ProfessionName = await _context.GetProfessionNameFromId(item.ProfessionId),
                    Ecnr = item.Ecnr,
                    SourceFrom = item.SourceFrom,
                    ContractReviewItemQs = itemQs,
                    RequireAssess = requireAssess
               });
          }

          contractReview = new ContractReview
          {
               OrderNo = order.OrderNo,
               OrderId = orderId,
               OrderDate = order.OrderDate,
               CustomerId = order.CustomerId,
               CustomerName = await _context.CustomerNameFromId(order.CustomerId),
               ReviewedByName = Username,
               ReviewedOn = System.DateTime.Now,
               ContractReviewItems = contractReviewItems
          };

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
     public async Task<bool> UpdateOrderReviewStatus(int orderId)
     {
          //if any item is not reviewed, return false
          var order = await _context.Orders.Include(x => x.OrderItems).Where(x => x.Id == orderId)
               .FirstOrDefaultAsync();

          var orderitemIds = order.OrderItems.Select(x => x.Id).ToList();

          var reviewitems = await _context.ContractReviewItems
               .Where(x => orderitemIds.Contains(x.OrderItemId))
               .Include(x => x.ContractReviewItemQs.Where(x => x.ReviewParameter=="Service Charges in INR"))
               .ToListAsync();
          
          if(reviewitems == null || reviewitems.Count == 0 || orderitemIds.Count != reviewitems.Count) {
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

       
    }
}