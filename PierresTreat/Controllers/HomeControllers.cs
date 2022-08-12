using Microsoft.AspNetCore.Mvc;

namespace PierresTreat.Controllers
{
    public class HomeController : Controller
    {

      [HttpGet("/")]
      public ActionResult Index()
      {
        return View();
      }

    }
}