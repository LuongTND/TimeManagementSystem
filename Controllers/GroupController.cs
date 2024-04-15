using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeTrackingSystem.Data;
using TimeTrackingSystem.Models;

namespace TimeTrackingSystem.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private readonly SystemContext _context;

        public GroupController(SystemContext context)
        {
            _context = context;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var id = int.Parse(User.FindFirstValue("Id"));
            var acc = _context.Accounts.Find(id);

            var joinedId = acc.JoinGroups.Select(g => g.GroupID);

            return View(await _context.Groups.Where(g => g.LeaderID != id && !joinedId.Contains(g.GroupID)).Include(g => g.Leader).ToListAsync());
        }

		// GET: Your Groups
		[Authorize(Policy = "Premium")]
		public async Task<IActionResult> YourGroup()
        {
            var id = int.Parse(User.FindFirstValue("Id"));
            return View(await _context.Groups.Where(g => g.LeaderID == id).Include(g => g.Leader).ToListAsync());
        }

        // GET: Joined Groups
        public async Task<IActionResult> JoinedGroup()
        {
            var id = int.Parse(User.FindFirstValue("Id"));
            var account = _context.Accounts.Find(id);
            return View(account.JoinGroups.Select(jgroup => jgroup.Group));
        }

		// GET: Groups/Create
		[Authorize(Policy = "Premium")]
		public IActionResult Create()
        {
            ViewData["LeaderID"] = new SelectList(_context.Accounts, "AccountID", "Name");
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Policy = "Premium")]
		public async Task<IActionResult> Create([Bind("GroupID,Name,LeaderID")] Group group)
        {
            if (ModelState.IsValid)
            {
                _context.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeaderID"] = new SelectList(_context.Accounts, "AccountID", "Name", group.LeaderID);
            return View(group);
        }

		// GET: Groups/Edit/5
		[Authorize(Policy = "Premium")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var group = await _context.Groups.FindAsync(id); if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Policy = "Premium")]
		public async Task<IActionResult> Edit(int id, [Bind("GroupID,Name,LeaderID")] Group group)
        {
            if (id != group.GroupID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(group.GroupID))
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
            return View(group);
        }


		// GET: Groups/Delete/5
		[Authorize(Policy = "Premium")]
		public async Task<IActionResult> Delete(int? id)
        {
            if (_context.Groups == null)
            {
                return Problem("Entity set 'SystemContext.Groups'  is null.");
            }
            var group = await _context.Groups.FindAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Delete group successfully!";
            return RedirectToAction(nameof(YourGroup));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.GroupID == id);
        }

        // GET: Groups/Join
        [HttpGet]
        [Route("Group/Join/{id?}")]
        public IActionResult Join(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var groupMember = new GroupMember()
            {
                AccountID = int.Parse(User.FindFirstValue("Id")),
                GroupID = id ?? 0
            };

            _context.GroupMembers.Add(groupMember);
            _context.SaveChanges();

            TempData["Message"] = "Join successfully!";
            return RedirectToAction("Index");
        }

        // GET: Groups/Out
        [HttpGet]
        [Route("Group/Out/{id?}")]
        public IActionResult Out(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var accountId = int.Parse(User.FindFirstValue("Id"));
            var groupMember = _context.GroupMembers.SingleOrDefault(gm => gm.GroupID == id && gm.AccountID == accountId);

            if(groupMember == null)
            {
                return NotFound();
            }

            _context.Remove(groupMember);
            _context.SaveChanges();

            TempData["Message"] = "Out successfully!";
            return RedirectToAction("JoinedGroup");
        }
    }

}
