using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeTrackingSystem.Data;
using TimeTrackingSystem.Models;

namespace TimeTrackingSystem.Controllers
{
    public class ClockController : Controller
    {
        private readonly SystemContext _context;

        public ClockController(SystemContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var clockTime = new Clock
            {
                WorkingTime = 25,
                RestTime = 5
            };
            if (User.Identity.IsAuthenticated)
            {
                var id = int.Parse(User.FindFirstValue("Id"));
                var accountClock = _context.Clocks.FirstOrDefault(c => c.AccountId == id);
                if(accountClock != null)
                {
                    clockTime = accountClock;
                }
            }

            return View(clockTime);
        }
    }
}
