using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TimeTrackingSystem.Data;
using TimeTrackingSystem.Models;

namespace TimeTrackingSystem.Controllers
{
    public class HomeController : Controller
    {
        private SystemContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, SystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public enum EventType
        {
            meeting,
            pertask
        }

        public class EventObject
        {
            public int ID { get; set; }
            public EventType CalendarID { get; set; }
            public string Title { get; set; }
            public string? Body { get; set; }
            public string StartAt { get; set; }
            public string EndAt { get; set; }
            public string Attend { get; set; }
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Clock");
            }

            List<EventObject> events = new List<EventObject>();
            var id = int.Parse(User.FindFirstValue("Id"));
            var account = _context.Accounts.Find(id);

            if (account == null) return RedirectToAction("Index", "Clock");

            foreach(var task in account.Tasks)
            {
                events.Add(new EventObject
                {
                    ID = events.Count() + 1,
                    CalendarID = EventType.pertask,
                    Title = task.Title,
                    StartAt = task.StartAt.ToString("O"),
                    EndAt = task.DueAt.ToString("O"),
                    Attend = "Me"
                });
            }

            var groupIDs = new List<int>();
            groupIDs.AddRange(account.LeadGroups.Select(g => g.GroupID));
            groupIDs.AddRange(account.JoinGroups.Select(g => g.GroupID));

            var meetings = _context.Meetings.Where(m => groupIDs.Contains(m.GroupID));
            
            foreach(var meeting in meetings)
            {
                events.Add(new EventObject
                {
                    ID = events.Count() + 1,
                    CalendarID = EventType.meeting,
                    Title = $"Meeting of {meeting.Group.Name}",
                    Body = $"<a href=\"/meetings/room/{meeting.MeetingCode}\">Link</a>",
                    StartAt = meeting.OpenAt.ToString("O"),
                    EndAt = meeting.CloseAt.ToString("O"),
                    Attend = meeting.Group.Name
                });
            }

            return View(events);
        }
    }
}
