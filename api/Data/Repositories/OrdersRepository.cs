using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.Messages;
using api.Interfaces.Orders;
using api.Params.Orders;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IComposeMessagesAdminRepository _msgAdmRepo;
        private readonly IJDAndRemunRepository _jdandremunRepo;
        public OrdersRepository(DataContext context, IMapper mapper, 
            IComposeMessagesAdminRepository msgAdmRepo, IJDAndRemunRepository jdandremunRepo)
        {
            _jdandremunRepo = jdandremunRepo;
            _msgAdmRepo = msgAdmRepo;
            _mapper = mapper;
            _context = context;
        }
        
        public async Task<OrderItem> AddOrderItem(OrderItemToCreateDto dto)
            {
            var item = _mapper.Map<OrderItem>(dto);
            item.Status="NotStarted";
            item.ReviewItemStatus = "NotReviewed";

            _context.Entry(item).State = EntityState.Added;

            if(await _context.SaveChangesAsync() > 0) return item;

            return null;

            }

        public async Task<MessageWithError> ComposeMsg_AckToClient(int orderid)
        {
            var order = await _context.Orders.FindAsync(orderid);
            var msgs = await _msgAdmRepo.AckEnquiryToCustomer(order);
            if(!string.IsNullOrEmpty(msgs.ErrorString) && msgs.Messages != null) {
                foreach(var msg in msgs.Messages) {
                    _context.Entry(msg).State = EntityState.Added;
                }
                await _context.SaveChangesAsync();
            }
            return msgs;
        }

        public async Task<Order> CreateOrderAsync(OrderToCreateDto dto)
        {
            var orders = new List<Order>();
            var orderNo = await _context.GetMaxOrderNo();

            var items = new List<OrderItem>();
            foreach (var item in dto.OrderItems)
            {
                    items.Add(new OrderItem{SrNo =item.SrNo, ProfessionId=item.ProfessionId, SourceFrom=item.SourceFrom, 
                    Quantity = item.Quantity, MinCVs = item.MinCVs, MaxCVs = item.MaxCVs, Ecnr = item.Ecnr, 
                    CompleteBefore=item.CompleteBefore, JobDescription=item.JobDescription, Remuneration=item.Remuneration});
            }

            // create order
            var order = new Order{OrderNo=++orderNo, CustomerId=dto.CustomerId, 
                CityOfWorking=dto.CityOfWorking,Country = dto.Country, OrderRef=dto.OrderRef,
                OrderRefDate=dto.OrderRefDate, CompleteBy=dto.CompleteBy, OrderItems=items};
            
            _context.Orders.Add(order);

            var result = await _context.SaveChangesAsync();
            if (result <= 0) return null;
            return order;
        }

        public async Task<bool> DeleteOrder(int orderid)
        {
            var order = await _context.Orders.FindAsync(orderid);
            if (order == null) return false;

            _context.Orders.Remove(order);
            _context.Entry(order).State = EntityState.Deleted;
            
            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            
            return  true;
        }

        public async Task<bool> DeleteOrderItem(int orderItemId)
        {
            var item = await _context.OrderItems.FindAsync(orderItemId);
            if (item == null) return false;
            _context.OrderItems.Remove(item);
            _context.Entry(item).State = EntityState.Deleted;
            
            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            
            return  true;
        }

        public async Task<bool> EditOrder(Order newObject)
        {
            var existingObject = _context.Orders
                .Where(x => x.Id == newObject.Id)
                .Include(x => x.OrderItems).ThenInclude (x => x.JobDescription)
                .Include(x => x.OrderItems).ThenInclude(x => x.Remuneration)
                .AsNoTracking()
                .SingleOrDefault();
            
            if (existingObject == null) return false;

            _context.Entry(existingObject).CurrentValues.SetValues(newObject);

            //delete records in existingObject that are not present in new object
            foreach (var existingItem in existingObject.OrderItems.ToList())
            {
                //consider calling EditOrderItem sub, instead of repeating following
                //it was not working, hence continuued with the repetitious code below
                if(!newObject.OrderItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.OrderItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
                
            }
            
            //items in current object - either updated or new items
            foreach(var newItem in newObject.OrderItems)
            {
                var existingItem = existingObject.OrderItems
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new OrderItem
                    {
                        OrderId = existingObject.Id,
                        ProfessionId = newItem.ProfessionId,
                        Quantity = newItem.Quantity,
                        MinCVs = newItem.MinCVs,
                        MaxCVs = newItem.MaxCVs,
                        SourceFrom = newItem.SourceFrom,
                        ReviewItemStatus = "NotReviewed",
                        Ecnr = newItem.Ecnr,
                        CompleteBefore = newItem.CompleteBefore,
                        Status = "Not Started",
                        JobDescription = _jdandremunRepo.CreateNewJobDescription(newItem.JobDescription,0),
                        Remuneration = _jdandremunRepo.CreateNewRemuneration(newItem.Remuneration, 0)
                    };

                    existingObject.OrderItems.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }

                //jobdescription
                if(existingItem != null) {
                    if(existingItem.JobDescription != null) {
                        var existingSubItem = existingItem.JobDescription;
                        if(newItem.JobDescription.Id != existingSubItem?.Id && newItem.JobDescription.Id != default(int))
                        {
                            _context.JobDescriptions.Remove(existingSubItem);
                            _context.Entry(existingSubItem).State = EntityState.Deleted; 
                        }
                    
                        if(existingSubItem != null)    //update navigation record
                        {
                            _context.Entry(existingSubItem).CurrentValues.SetValues(newItem.JobDescription);
                            _context.Entry(existingSubItem).State = EntityState.Modified;
                        } else {    //insert new navigation record
                            var itemToInsert = _jdandremunRepo.CreateNewJobDescription(newItem.JobDescription, existingItem.Id);

                            _context.Entry(itemToInsert).State = EntityState.Added;
                        }

                        //remuneration
                        var existingSubItem2 = existingItem.Remuneration;
                        if(newItem.Remuneration.Id != existingSubItem2?.Id && newItem.Remuneration.Id != default(int))
                        {
                            _context.Remunerations.Remove(existingSubItem2);
                            _context.Entry(existingSubItem2).State = EntityState.Deleted; 
                        }

                        if(existingSubItem2 != null)    //update navigation record
                        {
                            _context.Entry(existingSubItem2).CurrentValues.SetValues(newItem.Remuneration);
                            _context.Entry(existingSubItem2).State = EntityState.Modified;
                        } else {    //insert new navigation record
                            var remunerationToInsert = _jdandremunRepo.CreateNewRemuneration(newItem.Remuneration, existingItem.Id);
                            _context.Entry(remunerationToInsert).State = EntityState.Added;
                        }
                    }   
                }
            }
            
            _context.Entry(existingObject).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return true;
        }
        
        public async Task<bool> EditOrderItem(OrderItem newObject, bool DoNotSave)
        {
            var existingItem = await _context.OrderItems
                .Include(x => x.JobDescription)
                .Include(x => x.Remuneration)
                .Where(x => x.Id == newObject.Id)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if(newObject.Id != existingItem.Id && newObject.Id != default(int))
                {
                    _context.OrderItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            
            if(_context.Entry(existingItem).State != EntityState.Deleted) {
                _context.Entry(existingItem).CurrentValues.SetValues(newObject);
            } else {
                var itemToInsert = new OrderItem
                    {
                        OrderId = newObject.Id,
                        ProfessionId = newObject.ProfessionId,
                        Quantity = newObject.Quantity,
                        MinCVs = newObject.MinCVs,
                        MaxCVs = newObject.MaxCVs,
                        SourceFrom = newObject.SourceFrom,
                        ReviewItemStatus = "NotReviewed",
                        Ecnr = newObject.Ecnr,
                        CompleteBefore = newObject.CompleteBefore,
                        Status = "Not Started",
                    };

                    _context.Entry(itemToInsert).State = EntityState.Added;

                //jobdescription
                var existingSubItem = existingItem.JobDescription;
                 if(newObject.JobDescription.Id != existingSubItem.Id && newObject.JobDescription.Id != default(int))
                {
                    _context.JobDescriptions.Remove(existingSubItem);
                    _context.Entry(existingSubItem).State = EntityState.Deleted; 
                }

                if(existingSubItem != null)    //update navigation record
                {
                    _context.Entry(existingSubItem).CurrentValues.SetValues(newObject.JobDescription);
                    _context.Entry(existingSubItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var JDToInsert = new JobDescription
                    {
                        OrderItemId = existingItem.Id,
                        JobDescInBrief = newObject.JobDescription.JobDescInBrief,
                        MaxAge=newObject.JobDescription.MaxAge,
                        MinAge=newObject.JobDescription.MinAge,
                        ExpDesiredMin = newObject.JobDescription.ExpDesiredMin,
                        ExpDesiredMax = newObject.JobDescription.ExpDesiredMax,
                        QualificationDesired = newObject.JobDescription.QualificationDesired,
                    };
                    _context.JobDescriptions.Add(JDToInsert);
                    _context.Entry(JDToInsert).State = EntityState.Added;
                }

                //remuneration
                var existingSubItem2 = existingItem.Remuneration;
                 if(newObject.Remuneration.Id != existingSubItem2.Id && newObject.Remuneration.Id != default(int))
                {
                    _context.Remunerations.Remove(existingSubItem2);
                    _context.Entry(existingSubItem2).State = EntityState.Deleted; 
                }

                if(existingSubItem2 != null)    //update navigation record
                {
                    _context.Entry(existingSubItem2).CurrentValues.SetValues(newObject.Remuneration);
                    _context.Entry(existingSubItem2).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert2 = new Remuneration
                    {
                        OrderItemId = existingItem.Id,
                        WorkHours = newObject.Remuneration.WorkHours,
                        SalaryCurrency = newObject.Remuneration.SalaryCurrency,
                        SalaryMin = newObject.Remuneration.SalaryMin,
                        SalaryMax = newObject.Remuneration.SalaryMax,
                        ContractPeriodInMonths = newObject.Remuneration.ContractPeriodInMonths,
                        HousingProvidedFree = newObject.Remuneration.HousingProvidedFree,
                        HousingAllowance = newObject.Remuneration.HousingAllowance,
                        HousingNotProvided = newObject.Remuneration.HousingNotProvided,
                        FoodProvidedFree = newObject.Remuneration.FoodProvidedFree,
                        FoodAllowance = newObject.Remuneration.FoodAllowance,
                        FoodNotProvided = newObject.Remuneration.FoodNotProvided,
                        TransportProvidedFree = newObject.Remuneration.TransportProvidedFree,
                        TransportAllowance = newObject.Remuneration.TransportAllowance,
                        TransportNotProvided = newObject.Remuneration.TransportNotProvided,
                        OtherAllowance = newObject.Remuneration.OtherAllowance,
                        LeavePerYearInDays = newObject.Remuneration.LeavePerYearInDays,
                        LeaveAirfareEntitlementAfterMonths = newObject.Remuneration.LeaveAirfareEntitlementAfterMonths
                    };
                    _context.Remunerations.Add(itemToInsert2);

                }
            }
            _context.Entry(existingItem).State = EntityState.Modified;

            if (!DoNotSave) {
                try {
                    await _context.SaveChangesAsync();
                } catch (Exception ex) {
                    throw new Exception(ex.Message, ex);
                }
            } 
            return true;
        }

        public async Task<ICollection<OrderItemBriefDto>> GetOpenOrderItemsMatchingAProfession(int professionId)
        {
            var items = await _context.OrderItems
                .Where(x => x.ProfessionId == professionId && x.Status.ToLower() != "closed")
                .ProjectTo<OrderItemBriefDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            if(items == null || items.Count == 0) return null;

            foreach(var item in items)
            {
                item.ProfessionName = await _context.GetProfessionNameFromId(item.ProfessionId);
            }
            return items;
        }

        public async Task<PagedList<OrderItemBriefDto>> GetOpenOrderItems(OpenOrderItemsParams orderParams)
        {
            var query = (from item in _context.OrderItems
                join order in _context.Orders on item.OrderId equals order.Id 
                select new OrderItemBriefDto {
                    OrderId = order.Id,  AboutEmployer = order.Customer.Introduction,
                    CompleteBefore = item.CompleteBefore, CustomerId = order.CustomerId,
                    CustomerName = order.Customer.CustomerName, OrderItemId = item.Id,
                    JobDescription = item.JobDescription, OrderDate = order.OrderDate,
                    OrderNo = order.OrderNo, Ecnr = item.Ecnr, ProfessionId = item.ProfessionId,
                    ProfessionName = item.Profession.ProfessionName, Quantity = item.Quantity,
                    Remuneration = item.Remuneration, SrNo = item.SrNo, Status = item.Status 
                }).AsQueryable();
            
            if(orderParams.OrderId != 0) query = query.Where(x => x.OrderId == orderParams.OrderId);
            if(orderParams.OrderItemIds.Count > 0) 
                query = query.Where(x => orderParams.OrderItemIds.Contains(x.OrderItemId));
            if(orderParams.ProfessionIds.Count > 0)
                query = query.Where(x => orderParams.ProfessionIds.Contains(x.ProfessionId));
            if(orderParams.CustomerId != 0) query = query.Where(x => x.CustomerId == orderParams.CustomerId);

            var paged = await PagedList<OrderItemBriefDto>.CreateAsync(query.AsNoTracking()
                .ProjectTo<OrderItemBriefDto>(_mapper.ConfigurationProvider),
                orderParams.PageNumber, orderParams.PageSize);

            if(paged == null || paged.Count == 0) return null;

            foreach(var item in paged)
            {
                if(string.IsNullOrEmpty(item.ProfessionName)) item.ProfessionName = await _context.GetProfessionNameFromId(item.ProfessionId);
            }
            return paged;
        }

        public async Task<ICollection<OrderItemBriefDto>> GetOpenOrderItemsBriefDto()
        {
               var qry = await (from order in _context.Orders where order.Status != "Completed"
                        join item in _context.OrderItems on order.Id equals item.OrderId
                    select new OrderItemBriefDto {
                        OrderId = order.Id, OrderNo = order.OrderNo, OrderDate = order.OrderDate,
                        AboutEmployer = order.Customer.Introduction,
                        CustomerId = order.CustomerId, CustomerName = order.Customer.CustomerName,
                        OrderItemId = item.Id, SrNo = item.SrNo, ProfessionId = item.ProfessionId,
                        ProfessionName = item.Profession.ProfessionName, Quantity = item.Quantity,
                        Ecnr = item.Ecnr, CompleteBefore = item.CompleteBefore,
                        JobDescription = item.JobDescription, Remuneration = item.Remuneration,
                        Status = item.Status
                    }).ToListAsync();
                
            return qry;
        }

        public async Task<ICollection<OrderItemBriefDto>> GetOrderByIdWithItemsAsyc(int orderid)
        {
               var qry = await (from order in _context.Orders where order.Id == orderid
                        join item in _context.OrderItems on order.Id equals item.OrderId
                    select new OrderItemBriefDto {
                        OrderId = orderid, OrderNo = order.OrderNo, OrderDate = order.OrderDate,
                        AboutEmployer = order.Customer.Introduction,
                        CustomerId = order.CustomerId, CustomerName = order.Customer.CustomerName,
                        OrderItemId = item.Id, SrNo = item.SrNo, ProfessionId = item.ProfessionId,
                        ProfessionName = item.Profession.ProfessionName, Quantity = item.Quantity,
                        Ecnr = item.Ecnr, CompleteBefore = item.CompleteBefore,
                        JobDescription = item.JobDescription, Remuneration = item.Remuneration,
                        Status = item.Status
                    }).ToListAsync();
                
            return qry;
        }

        public async Task<OrderItemBriefDto> GetOrderItemBrief(int orderitemid)
        {
               var qry = await (from order in _context.Orders
                        join item in _context.OrderItems on order.Id equals item.OrderId
                            where item.Id == orderitemid
                    select new OrderItemBriefDto {
                        OrderId = order.Id, OrderNo = order.OrderNo, OrderDate = order.OrderDate,
                        AboutEmployer = order.Customer.Introduction,
                        CustomerId = order.CustomerId, CustomerName = order.Customer.CustomerName,
                        OrderItemId = item.Id, SrNo = item.SrNo, ProfessionId = item.ProfessionId,
                        ProfessionName = item.Profession.ProfessionName, Quantity = item.Quantity,
                        Ecnr = item.Ecnr, CompleteBefore = item.CompleteBefore,
                        JobDescription = item.JobDescription, Remuneration = item.Remuneration,
                        Status = item.Status
                    }).FirstOrDefaultAsync();
                
            return qry;
        }

        public async Task<OrderDisplayDto> GetOrderByIdWithAllRelatedProperties(int id)
        {
            var order = await _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.JobDescription)
                .Include(x => x.OrderItems).ThenInclude(x => x.Remuneration)
                .Where(x => x.Id == id)
                .ProjectTo<OrderDisplayDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            if(order==null) return null;

            order.ProjectManagerName = await _context.GetEmployeeNameFromId(order.ProjectManagerId);
            order.CustomerName = await _context.CustomerNameFromId(order.CustomerId);
            foreach(var item in order.OrderItems)
            {
                item.ProfessionName = await _context.GetProfessionNameFromId(item.ProfessionId);
            }
            
            return order;
        }

        public async Task<string> GetOrderItemRefCode(int orderitemid)
        {
            var obj = await(from order in _context.Orders
                join item in _context.OrderItems on order.Id equals item.OrderId
                        where item.Id == orderitemid
                select order.OrderNo + "-" + item.SrNo + "-" + item.Profession.ProfessionName
            ).FirstOrDefaultAsync();
        
            return obj;
        }
        public async Task<PagedList<OrderBriefDto>> GetOrdersAllAsync(OrdersParams orderParams)
        {
            var query = _context.Orders.AsQueryable();

            if(orderParams.Id > 0) {
                query = query.Where(x => x.Id==orderParams.Id);
            } else if(orderParams.OrderNo > 0) {
                query = query.Where(x => x.OrderNo == orderParams.OrderNo);
            } else {
                if(orderParams.CustomerId > 0) query = query.Where(x => x.CustomerId == orderParams.CustomerId);
            }

            var paged = await PagedList<OrderBriefDto>.CreateAsync(query.AsNoTracking()
                .ProjectTo<OrderBriefDto>(_mapper.ConfigurationProvider),
                orderParams.PageNumber, orderParams.PageSize);
            
            return paged;
        }

        public async Task<PagedList<OpenOrderItemCategoriesDto>> GetOpenItemCategories(OpenOrderItemsParams orderParams)
        {
            var query = (from item in _context.OrderItems
                join order in _context.Orders on item.OrderId equals order.Id 
                select new OpenOrderItemCategoriesDto {
                    OrderItemId = item.Id, OrderDate = order.OrderDate,
                    CustomerName = order.Customer.CustomerName,
                    OrderNo = order.OrderNo, Quantity = item.Quantity,
                    CategoryRefAndName = order.OrderNo + "-" + item.SrNo + "-" + item.Profession.ProfessionName
                }).AsQueryable();
            
            if(orderParams.OrderId != 0) query = query.Where(x => x.OrderId == orderParams.OrderId);
            if(orderParams.OrderItemIds.Count > 0) 
                query = query.Where(x => orderParams.OrderItemIds.Contains(x.OrderItemId));
            //if(orderParams.ProfessionIds.Count > 0)
                //query = query.Where(x => orderParams.ProfessionIds.Contains(x.ProfessionId));
            //if(orderParams.CustomerId != 0) query = query.Where(x => x.CustomerId == orderParams.CustomerId);

            var paged = await PagedList<OpenOrderItemCategoriesDto>.CreateAsync(query.AsNoTracking()
                .ProjectTo<OpenOrderItemCategoriesDto>(_mapper.ConfigurationProvider),
                orderParams.PageNumber, orderParams.PageSize);

            if(paged == null || paged.Count == 0) return null;

            return paged;
        }

        public async Task<ICollection<OpenOrderItemCategoriesDto>> GetOpenItemCategoryList()
        {
            var query = await (from item in _context.OrderItems 
                where item.Status != "Completed" orderby item.SrNo
                join order in _context.Orders on item.OrderId equals order.Id 
                    where order.Status != "Completed" orderby order.OrderNo
                join assmtItem in _context.OrderAssessmentItems on item.Id equals assmtItem.OrderItemId
                join rvwitem in _context.ContractReviewItems on item.Id equals rvwitem.OrderItemId 
                //into rvwGroup
                //from rg in rvwGroup.DefaultIfEmpty()
                //join prof in _context.Professions on item.ProfessionId equals prof.Id
                select new OpenOrderItemCategoriesDto {
                    OrderItemId = item.Id, OrderDate = order.OrderDate,
                    CustomerName = order.Customer.CustomerName,
                    OrderNo = order.OrderNo, Quantity = item.Quantity,
                    RequireInternalReview = rvwitem.ContractReviewItemQs.Count > 0,
                    AssessmentQDesigned = assmtItem.OrderAssessmentItemQs.Count > 0,
                    CategoryRefAndName = order.OrderNo + "-" + item.SrNo + "-" + item.Profession.ProfessionName
                }).ToListAsync();

            return query;
        }

    }
}
