using api.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class CVRefExtensions
    {
  
        public async static Task<bool> CVRefIsEditable(this DataContext dataContext, int cvrefid)
        {
            var refStatus = await dataContext.CVRefs
               .Where(x => x.Id == cvrefid).Select(x => x.RefStatus)
               .FirstOrDefaultAsync();

            if(refStatus.ToLower()["referred".IndexOf("referred")..]!="") return false;
                //status is no longer not referred or referred, client has made a decision on it, so the CVRef cannot be changed
            return true;
        }
    }
}