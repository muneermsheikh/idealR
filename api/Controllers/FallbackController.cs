using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class FallbackController: Controller     //publication
    {
        public ActionResult Index() {

            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
                "wwwroot", "index.html" ), "text/HTML");

        }
    }
}