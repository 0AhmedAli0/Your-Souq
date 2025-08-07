using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    public class FallbackController : Controller
    {
        //this controller is used to handle the requests that are not handled by any other controller
        //it will return the index.html file of the angular project
        [HttpGet]
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/html");
        }
    }
    
    
}
