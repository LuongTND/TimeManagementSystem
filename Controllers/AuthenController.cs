using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeTrackingSystem.Data;
using TimeTrackingSystem.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Data;


namespace TimeTrackingSystem.Controllers
{
    public class AuthenController : Controller
    {
        private readonly SystemContext _context;

        public AuthenController(SystemContext context)
        {
            _context = context;
        }

		[Authorize]
		public IActionResult UpdateAccountPre()
        {
            return View();
        }

        public async System.Threading.Tasks.Task LoginGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var claims = result.Principal;

            var emailClaim = result.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (emailClaim?.Value != null)
            {

                var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == emailClaim.Value);

                if (existingUser == null)
                {

                    existingUser = new Account()
                    {
                        Name = claims.FindFirstValue(ClaimTypes.Name),
                        Email = claims.FindFirstValue(ClaimTypes.Email),
                        RoleID = 1
                    };

                    _context.Add(existingUser);
                    await _context.SaveChangesAsync();
                  
                }
               

                List<Claim> claim = new List<Claim>()
                {
                    new Claim("Id", existingUser.AccountID.ToString()),
                    new Claim(ClaimTypes.Name,existingUser.Name),
                    new Claim(ClaimTypes.Role, existingUser.RoleID.ToString())
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            }

            return RedirectToAction("Index", "Clock");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Clock");
        }

        [Authorize]
        public async Task<IActionResult> UpdateAccountPremium()
        {
            var id = int.Parse(User.FindFirstValue("Id"));

            var account = _context.Accounts.Find(id);
            account.RoleID = 2;
            _context.SaveChanges();

            List<Claim> claim = new List<Claim>()
                {
                    new Claim("Id", account.AccountID.ToString()),
                    new Claim(ClaimTypes.Name,account.Name),
                    new Claim(ClaimTypes.Role, account.RoleID.ToString())
                };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(1)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            TempData["Message"] = "Update role successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}
