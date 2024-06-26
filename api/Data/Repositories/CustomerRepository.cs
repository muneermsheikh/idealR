using api.DTOs.Admin;
using api.DTOs.Customer;
using api.Entities.Admin.Client;
using api.Entities.Identity;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public CustomerRepository(DataContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }


        public async Task<bool> DeleteCustomer(int CustomerId)
        {
            var customer = await _context.Customers.FindAsync(CustomerId);
            if (customer == null) return false;
            _context.Customers.Remove(customer);
            _context.Entry(customer).State = EntityState.Deleted;
            try{
                await _context.SaveChangesAsync();
            } catch(Exception ex) {
                throw new Exception(ex.Message);
            }
            
            return true;
        
        }
        
        public async Task<ICollection<string>> GetCustomerCities(string customerType) {

            var cities = await _context.Customers.Where(x => x.CustomerType.ToLower() == customerType.ToLower())
                .Select(x => x.City).ToListAsync();
            
            return cities;
        }

        public async Task<Entities.Admin.Client.Customer> GetCustomerById(int Id)
        {
            var cust = await _context.Customers
                .Include(x => x.CustomerOfficials)
                .FirstOrDefaultAsync(x => x.Id == Id);
            
            return cust;
        }

        public async Task<Entities.Admin.Client.Customer> GetCustomer(CustomerParams customerParams)
        {
            var query = _context.Customers.AsQueryable();
            if(customerParams.CustomerId > 0) {
                query = query.Where(x => x.Id == customerParams.CustomerId);}
                else {
                    if(!string.IsNullOrEmpty(customerParams.CustomerType)) 
                        query = query.Where(x => x.CustomerType.ToLower() == customerParams.CustomerType.ToLower());
                    if(!string.IsNullOrEmpty(customerParams.CustomerName))
                        query = query.Where(x => x.CustomerName.ToLower() == customerParams.CustomerName.ToLower());
                    if(!string.IsNullOrEmpty(customerParams.customerCity))
                        query = query.Where(x => x.City.ToLower()==customerParams.customerCity.ToLower());
                }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<PagedList<CustomerDto>> GetCustomers(CustomerParams customerParams)
        {
            var query = _context.Customers.AsQueryable();

            if(customerParams.CustomerId > 0) {
                query = query.Where(x => x.Id == customerParams.CustomerId);}
                else {
                    if(!string.IsNullOrEmpty(customerParams.CustomerType)) 
                        query = query.Where(x => x.CustomerType.ToLower() == customerParams.CustomerType.ToLower());
                    if(!string.IsNullOrEmpty(customerParams.CustomerName))
                        query = query.Where(x => x.CustomerName.ToLower() == customerParams.CustomerName.ToLower());
                    if(!string.IsNullOrEmpty(customerParams.customerCity))
                        query = query.Where(x => x.City.ToLower()==customerParams.customerCity.ToLower());
                }
            
            var paged = await PagedList<CustomerDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider),
                    customerParams.PageNumber, customerParams.PageSize);
            
            return paged;
        }

        public async Task<bool> InsertCustomer(CreateCustomerDto customer)
        {
            
            foreach(var official in customer.CustomerOfficials) {
                official.AppUserId = await UpdateAppUserIdExtensions.UpdateCustomerOfficialAppUserId(official, 
                    _userManager, _context, customer.City );
            };

            var cust = _mapper.Map<Entities.Admin.Client.Customer>(customer);
            var offs = customer.CustomerOfficials;
            cust.CustomerOfficials=offs;
            _context.Customers.Add(cust);
            
            return await _context.SaveChangesAsync() > 0;
            
        }

        public async Task<bool> UpdateCustomer(Entities.Admin.Client.Customer newObject)
        {
            var existingObject = _context.Customers
                .Where(x => x.Id == newObject.Id)
                .Include(x => x.CustomerOfficials)
                .Include(x => x.CustomerIndustries)
                .Include(x => x.AgencySpecialties)
                .AsNoTracking()
                .SingleOrDefault();
            
            if (existingObject == null) return false;

            _context.Entry(existingObject).CurrentValues.SetValues(newObject);

            //delete records in existingObject that are not present in new object
            foreach (var existingItem in existingObject.CustomerOfficials.ToList())
            {
                if(!newObject.CustomerOfficials.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.CustomerOfficials.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in newObject.CustomerOfficials)
            {
                var existingItem = existingObject.CustomerOfficials
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    var newAppUser = await CreateAppUserForCustomerOfficial(newItem);
                    if(newAppUser != null) newItem.AppUserId=newAppUser.Id;
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                        
                    var newAppUser = await CreateAppUserForCustomerOfficial(newItem);
                    if(newAppUser == null) continue;
                    var itemToInsert = new CustomerOfficial
                    {
                        AppUserId = newAppUser.Id,
                        CustomerId = existingObject.Id,
                        OfficialName = newItem.OfficialName,
                        Designation = newItem.Designation,
                        Divn = newItem.Divn,
                        Email = newItem.Email,
                        PhoneNo = newItem.PhoneNo,
                        Mobile = newItem.Mobile,
                        Gender = newItem.Gender,
                        Title = newItem.Title,
                        KnownAs = newItem.KnownAs,
                        UserName = newItem.UserName,
                        Status = newItem.Status
                    };

                    existingObject.CustomerOfficials.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

        if(newObject.CustomerType.ToLower() == "agent")
        {
            //delete records in existingObject that are not present in new object
            foreach (var existingItem in existingObject.AgencySpecialties.ToList())
            {
                if(!newObject.CustomerOfficials.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.AgencySpecialties.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }
            
            foreach(var newItem in newObject.AgencySpecialties)
            {
                var existingItem = existingObject.AgencySpecialties
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new AgencySpecialty
                    {
                        CustomerId = existingObject.Id,
                        ProfessionId = newItem.ProfessionId,
                        IndustryId = newItem.IndustryId,
                        SpecialtyName = newItem.SpecialtyName
                    };

                    existingObject.AgencySpecialties.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }
        }
        
            _context.Entry(existingObject).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<AppUser> CreateAppUserForCustomerOfficial(CustomerOfficial official) {
            var appUserData = new AppUser{
                UserName = official.UserName, Gender=official.Gender, Email=official.Email,
                KnownAs=official.KnownAs, PhoneNumber=official.Mobile, Position=official.Designation
            };
            var userAdded =await _userManager.CreateAsync(appUserData, "Pa$$w0rd");
            if(!userAdded.Succeeded) return null;
            await _userManager.AddToRoleAsync(appUserData, "Client");

            return appUserData;
        }

        public async Task<bool> UpdateCustomerOfficialWithAppuserId(CustomerOfficial official) {
            if(official.AppUserId != 0) return false;

            var user = await CreateAppUserForCustomerOfficial(official);
            if(user != null) {
                official.AppUserId=user.Id;
                _context.Entry(official).State = EntityState.Modified;

                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        Task<Entities.Admin.Client.Customer> ICustomerRepository.GetCustomer(CustomerParams customerParams)
        {
            throw new NotImplementedException();
        }

        Task<Entities.Admin.Client.Customer> ICustomerRepository.GetCustomerById(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<ClientIdAndNameDto>> GetCustomerIdAndNames(string customerType)
        {
            var obj = await _context.Customers.Where(x => x.CustomerType.ToLower() == customerType.ToLower())
                .Select(x => new ClientIdAndNameDto{
                     customerId=x.Id, CustomerName=x.CustomerName, KnownAs = x.KnownAs,
                }).ToListAsync();
            
            return obj;
        }

        public Task<bool> InsertCustomer(Entities.Admin.Client.Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<CustomerAndOfficialsDto>> GetCustomerAndOfficials(string customerType)
        {
            var obj = await _context.Customers.Include(x => x.CustomerOfficials)
                .Where(x => x.CustomerType.ToLower() == customerType.ToLower())
                .ProjectTo<CustomerAndOfficialsDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            
            return obj;
        }

        public async Task<CustomerOfficialDto> GetCustomerOfficialDto(int CustomerOfficialId)
        {
            var off = await (from c in _context.Customers
                    .Where(x => x.CustomerStatus.ToLower()=="active" )
                    join o in _context.CustomerOfficials on c.Id equals o.CustomerId 
                        where o.Id == CustomerOfficialId
                    select new CustomerOfficialDto{
                        AboutCompany = c.Introduction, City = c.City, CompanyKnownAs = c.KnownAs, Country = c.Country,
                        CustomerId = c.Id, CustomerIsBlacklisted = c.IsBlackListed, CustomerName = c.CustomerName, 
                        Title = o.Title, OfficialAppUserId = o.AppUserId, OfficialEmailId = o.Email, Designation=o.Designation,
                        OfficialId = o.Id, MobileNo = o.Mobile, OfficialName = o.OfficialName}).FirstOrDefaultAsync();
               
               return off;		
        }

        public async Task<ICollection<OfficialAndCustomerNameDto>> GetOfficialsAndCustomerNames(string customerType)
        {
            var query = await (from off in _context.CustomerOfficials
                join cust in _context.Customers on off.CustomerId equals cust.Id
                     where cust.CustomerType==customerType
                select new OfficialAndCustomerNameDto{
                    CustomerId = cust.Id, CustomerName=cust.CustomerName, Id=off.Id,
                    OfficialName = off.OfficialName + "-" + cust.KnownAs
                }).OrderBy(x => x.CustomerName).ThenBy(x => x.OfficialName)
                .ToListAsync();
            
            return query;
                
        }

        
    }
}