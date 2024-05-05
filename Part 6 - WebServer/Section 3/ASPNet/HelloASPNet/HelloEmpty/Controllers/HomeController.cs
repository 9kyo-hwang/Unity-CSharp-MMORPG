using HelloEmpty.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelloEmpty.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        HelloMessage msg = new HelloMessage
        {
            Message = "Welcome to ASP.Net MVC!"
        };

        ViewBag.Notification = "Input message and click to submit";
        
        return View(msg);
    }
}