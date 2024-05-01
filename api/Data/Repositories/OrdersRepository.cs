using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
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
        public OrdersRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<JobDescription> AddJobDescription(JobDescription jobDescription)
        {
            if(jobDescription.OrderItemId==0) return null;
            if(jobDescription.Id !=0) {
                var jd = await _context.JobDescriptions.FindAsync(jobDescription.Id);
                if(jd != null) return null;
            } 

            _context.Entry(jobDescription).State=EntityState.Added;

            if(await _context.SaveChangesAsync() > 0) return jobDescription;

            return null;
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

        public async Task<Remuneration> AddRemuneration(Remuneration remuneration)
        {
            if(remuneration.OrderItemId == 0) return null;

            _context.Entry(remuneration).State=EntityState.Added;   

            if(await _context.SaveChangesAsync() > 0) return remuneration;

            return null;
        }

        public Task<bool> ComposeMsg_AckToClient(int orderid)
        {
            throw new NotImplementedException();
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

        public async Task<bool> DeleteJobDescription(int jobDescriptionId)
        {
            var jd = await _context.JobDescriptions.FindAsync(jobDescriptionId);
            if (jd == null) return false;
            _context.Entry(jd).State = EntityState.Deleted;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOrder(int orderid)
        {
            var order = await _context.Orders.FindAsync(orderid);
            if (order == null) return false;
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
            var item = await _context.Orders.FindAsync(orderItemId);
            if (item == null) return false;
            _context.Entry(item).State = EntityState.Deleted;
            
            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            
            return  true;
        }

        public async Task<bool> DeleteRemuneration(int remunerationId)
        {
           var item = await _context.Remunerations.FindAsync(remunerationId);
            if (item == null) return false;
            _context.Entry(item).State = EntityState.Deleted;
            
            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            
            return  true;
        }

        public async Task<bool> EditJobDescription(JobDescription jobDescription)
        {
            var existingObject = await _context.JobDescriptions.FindAsync(jobDescription.Id);
            if (existingObject == null) return false;
            
            _context.Entry(existingObject).CurrentValues.SetValues(jobDescription);

            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return true;
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
                        JobDescription = CreateNewJobDescription(newItem.JobDescription,0),
                        Remuneration = CreateNewRemuneration(newItem.Remuneration, 0)
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
                            var itemToInsert = CreateNewJobDescription(newItem.JobDescription, existingItem.Id);

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
                            var remunerationToInsert = CreateNewRemuneration(newItem.Remuneration, existingItem.Id);
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
        
        private static Remuneration CreateNewRemuneration(Remuneration remun, int orderItemId)
        {
            var itemToInsert2 = new Remuneration
            {
                OrderItemId = orderItemId,
                WorkHours = remun.WorkHours,
                SalaryCurrency = remun.SalaryCurrency,
                SalaryMin = remun.SalaryMin,
                SalaryMax = remun.SalaryMax,
                ContractPeriodInMonths = remun.ContractPeriodInMonths,
                HousingProvidedFree = remun.HousingProvidedFree,
                HousingAllowance = remun.HousingAllowance,
                HousingNotProvided = remun.HousingNotProvided,
                FoodProvidedFree = remun.FoodProvidedFree,
                FoodAllowance = remun.FoodAllowance,
                FoodNotProvided = remun.FoodNotProvided,
                TransportProvidedFree = remun.TransportProvidedFree,
                TransportAllowance = remun.TransportAllowance,
                TransportNotProvided = remun.TransportNotProvided,
                OtherAllowance = remun.OtherAllowance,
                LeavePerYearInDays = remun.LeavePerYearInDays,
                LeaveAirfareEntitlementAfterMonths = remun.LeaveAirfareEntitlementAfterMonths
            };
            return itemToInsert2;
        }
        private static JobDescription CreateNewJobDescription(JobDescription newJD, int OrderItemId)
        {
            var itemToInsert = new JobDescription
                {
                    OrderItemId = OrderItemId,
                    JobDescInBrief = newJD.JobDescInBrief,
                    MaxAge=newJD.MaxAge,
                    MinAge=newJD.MinAge,
                    ExpDesiredMin = newJD.ExpDesiredMin,
                    ExpDesiredMax = newJD.ExpDesiredMax,
                    QualificationDesired = newJD.QualificationDesired,
                };
            return itemToInsert;
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

        public async Task<bool> EditRemuneration(Remuneration remuneration)
        {
            var existingObject = await _context.Remunerations.FindAsync(remuneration.Id);
            if (existingObject == null) throw new Exception("no such remuneration record");
            
            _context.Entry(existingObject).CurrentValues.SetValues(remuneration);

            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
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

        public async Task<OrderDisplayWithItemsDto> GetOrderByIdWithItemsAsyc(int id)
        {
               var order = await _context.Orders.Include(x => x.OrderItems)
                .Where(x => x.Id == id)
                .ProjectTo<OrderDisplayWithItemsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            order.ProjectManagerName = await _context.GetEmployeeNameFromId(order.ProjectManagerId);
            order.CustomerName = await _context.CustomerNameFromId(order.CustomerId);
            foreach(var item in order.OrderItems)
            {
                item.ProfessionName = await _context.GetProfessionNameFromId(item.ProfessionId);
            }
            
            return order;
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

        public async Task<JobDescription> GetJDOfOrderItem(int OrderItemId)
        {
            var jd = await _context.JobDescriptions.Where(x => x.OrderItemId == OrderItemId).FirstOrDefaultAsync();

            return jd;
        }

        public async Task<Remuneration> GetRemuneratinOfOrderItem(int OrderItemId)
        {
            var remun = await _context.Remunerations.Where(x => x.OrderItemId == OrderItemId).FirstOrDefaultAsync();

            return remun;
        }
    }
}
