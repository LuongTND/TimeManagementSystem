using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeTrackingSystem.Data;
using TimeTrackingSystem.Models;

namespace TimeTrackingSystem.Controllers
{
    [Authorize]
    public class MeetingsController : Controller
    {
        private readonly SystemContext _context;

        public MeetingsController(SystemContext context)
        {
            _context = context;
        }

        // GET: Meetings
        public async Task<IActionResult> Index()
        {
            var id = int.Parse(User.FindFirstValue("Id"));
            var acc = _context.Accounts.Find(id);
            var listGroupId = new List<int>();
            listGroupId.AddRange(acc.JoinGroups?.Select(g => g.GroupID) ?? new List<int>());
            listGroupId.AddRange(acc.LeadGroups?.Select(g => g.GroupID) ?? new List<int>());

            var systemContext = _context.Meetings.Where(m => listGroupId.Contains(m.GroupID)).Include(m => m.Group);
            return View(await systemContext.ToListAsync());
        }

		// GET: Meetings
		[Authorize(Policy = "Premium")]
		public async Task<IActionResult> GroupMeeting()
        {
            var id = int.Parse(User.FindFirstValue("Id"));
            var acc = _context.Accounts.Find(id);
            var listGroupId = new List<int>();
            listGroupId.AddRange(acc.LeadGroups?.Select(g => g.GroupID) ?? new List<int>());

            var systemContext = _context.Meetings.Where(m => listGroupId.Contains(m.GroupID)).Include(m => m.Group);
            return View(await systemContext.ToListAsync());
        }

		// GET: Meetings/Create
		[Authorize(Policy = "Premium")]
		public IActionResult Create()
        {
            var id = int.Parse(User.FindFirstValue("Id"));
            ViewData["GroupID"] = new SelectList(_context.Groups.Where(g => g.LeaderID == id), "GroupID", "Name");
            return View();
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Policy = "Premium")]
		public async Task<IActionResult> Create([Bind("MeetingCode,OpenAt,CloseAt,GroupID")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                meeting.MeetingCode = Guid.NewGuid();
                _context.Add(meeting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GroupMeeting));
            }
            return Problem("Something was wrong");
        }

		// GET: Meetings/Edit/5
		[Authorize(Policy = "Premium")]
		public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Meetings == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }
            var userid = int.Parse(User.FindFirstValue("Id"));

            ViewData["GroupID"] = new SelectList(_context.Groups.Where(g => g.LeaderID == userid), "GroupID", "Name");
            return View(meeting);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Policy = "Premium")]
		public async Task<IActionResult> Edit(Guid id, [Bind("MeetingCode,OpenAt,CloseAt,GroupID")] Meeting meeting)
        {
            if (id != meeting.MeetingCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meeting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeetingExists(meeting.MeetingCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var userid = int.Parse(User.FindFirstValue("Id"));

            ViewData["GroupID"] = new SelectList(_context.Groups.Where(g => g.LeaderID == userid), "GroupID", "Name");
            return RedirectToAction(nameof(GroupMeeting));
        }

		// GET: Meetings/Delete/5
		[Authorize(Policy = "Premium")]
		public async Task<IActionResult> Delete(Guid? id)
        {
            if (_context.Meetings == null)
            {
                return Problem("Entity set 'SystemContext.Meetings'  is null.");
            }
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting != null)
            {
                _context.Meetings.Remove(meeting);
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Cancel successfully!";
            return RedirectToAction(nameof(GroupMeeting));
        }

        private bool MeetingExists(Guid id)
        {
          return (_context.Meetings?.Any(e => e.MeetingCode == id)).GetValueOrDefault();
        }

        public IActionResult Room([FromRoute] string id = "")
        {
            var meeting = _context.Meetings.FirstOrDefault(m => m.MeetingCode.ToString() == id);

            if(meeting == null)
            {
                TempData["Message"] = "Meeting is not exist!";
                return RedirectToAction("Index", "Home");
            }

            var userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                TempData["Message"] = "Required authentication!";
                return RedirectToAction("Index", "Home");
            }

            var user = _context.Accounts.Find(int.Parse(userId));

            if((!user.JoinGroups?.Any(g => g.GroupID == meeting.GroupID) ?? true) 
                && (!user.LeadGroups?.Any(g => g.GroupID == meeting.GroupID) ?? true))
            {
                TempData["Message"] = "You can not join this meeting!";
                return RedirectToAction("Index", "Home");
            }

            if(meeting.OpenAt > DateTime.Now)
            {
                TempData["Message"] = "Meeting do not start now!";
                return RedirectToAction("Index", "Home");
            }

            if (meeting.CloseAt < DateTime.Now)
            {
                TempData["Message"] = "Meeting have finished already!";
                return RedirectToAction("Index", "Home");
            }

            return View("JoinRoom", id);
        }
    }
}
