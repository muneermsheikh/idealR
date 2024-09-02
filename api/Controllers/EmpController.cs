using api.Interfaces.Admin;

namespace api.Controllers
{
    public class EmpController : BaseApiController
    {
        private readonly IEmployeeRepository _empRepo;
        public EmpController(IEmployeeRepository empRepo)
        {
            _empRepo = empRepo;
        }

        

    }
}