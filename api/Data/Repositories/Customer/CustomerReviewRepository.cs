using api.Entities.Admin.Client;
using api.Interfaces.Customers;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Customer
{
    public class CustomerReviewRepository : ICustomerReviewRepository
    {
        private readonly DataContext _context;
        public CustomerReviewRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<bool> DeleteCustomerReview(int customerId)
        {
            var review = await _context.CustomerReviews.Where(x => x.CustomerId == customerId).FirstOrDefaultAsync();
            if(review == null) return false;

            _context.CustomerReviews.Remove(review);
            _context.Entry(review).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<CustomerReview> GetCustomerReview(int customerId)
        {
            var review =await _context.CustomerReviews
                .Include(x => x.CustomerReviewItems)
                .Where(x => x.CustomerId == customerId)
            .FirstOrDefaultAsync();

            return review;
        }

        public async Task<ICollection<string>> GetCustomerReviewStatusData()
        {
            var str = await _context.CustomerReviews.Select(x => x.CurrentStatus)
                .Distinct().ToListAsync();
            
            return str;
        }


        public async Task<bool> UpdateCustomerReview(CustomerReview model, string Username)
        {
            var existing = await _context.CustomerReviews
                .Include(x => x.CustomerReviewItems)
                .Where(x => x.Id == model.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if(existing==null) return false;

            _context.Entry(existing).CurrentValues.SetValues(model);

            //delete records in existingObject that are not present in new object
            foreach (var existingItem in existing.CustomerReviewItems.ToList())
            {
                if(!model.CustomerReviewItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.CustomerReviewItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in model.CustomerReviewItems)
            {
                var existingItem = existing.CustomerReviewItems
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                        
                    var itemToInsert = new CustomerReviewItem
                    {
                        ApprovedBySupUsername = newItem.ApprovedBySupUsername,
                        ApprovedOn = newItem.ApprovedOn, 
                        CustomerReviewDataId = newItem.CustomerReviewDataId,
                        CustomerReviewId = model.Id,
                        Remarks = newItem.Remarks,
                        ReviewTransactionDate = newItem.ReviewTransactionDate,
                        Username = Username
                    };

                    existing.CustomerReviewItems.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existing).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}