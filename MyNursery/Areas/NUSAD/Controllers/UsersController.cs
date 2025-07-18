using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNursery.Areas.NUSAD.Models;
using MyNursery.Areas.Welcome.Models;
using MyNursery.Data;
using MyNursery.Models;
using MyNursery.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUSAD.Controllers
{
    [Area("NUSAD")]
    [Authorize(Roles = SD.Role_SuperAdmin)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: NUSAD/Users/ManageUsers
        public async Task<IActionResult> ManageUsers()
        {
            // 1. Get only Registered users (exclude Predefined)
            var identityUsersList = await _userManager.Users
                .Where(u => u.UserType == SD.UserType_Registered)
                .ToListAsync();

            var identityEmails = identityUsersList.Select(u => u.Email.ToLower()).ToList();

            var identityUsersWithRoles = new List<UserDisplayViewModel>();

            foreach (var user in identityUsersList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                identityUsersWithRoles.Add(new UserDisplayViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    Email = user.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "",
                    ContactNumber = user.PhoneNumber,
                    DateCreated = user.DateCreated,
                    IsActive = user.IsActive,
                    LastLoginDate = user.LastLoginDate,
                    UserType = user.UserType
                });
            }

            // 2. Get AdminAdded custom users where Email is not already in Identity (avoid duplicates)
            var customUsers = await _context.Users
                .Where(u => !identityEmails.Contains(u.EmailAddress.ToLower()))
                .Select(u => new UserDisplayViewModel
                {
                    Id = u.Id.ToString(),
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.EmailAddress,
                    Role = u.Role,
                    ContactNumber = u.ContactNumber,
                    DateCreated = u.AddedDate,
                    IsActive = true,
                    LastLoginDate = null,
                    UserType = "AdminAdded"
                }).ToListAsync();

            // 3. Combine and return
            var combinedUsers = identityUsersWithRoles.Concat(customUsers).ToList();
            return View(combinedUsers);
        }

        // GET: NUSAD/Users/GetUserDetails/{id}
        [HttpGet]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            // Custom user
            if (int.TryParse(id, out int userId))
            {
                var user = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new UserDisplayViewModel
                    {
                        Id = u.Id.ToString(),
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.EmailAddress,
                        Role = u.Role,
                        ContactNumber = u.ContactNumber,
                        DateCreated = u.AddedDate,
                        IsActive = true,
                        LastLoginDate = null,
                        UserType = "AdminAdded"
                    }).FirstOrDefaultAsync();

                if (user != null)
                    return Json(user);
            }

            // Identity user
            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser != null)
            {
                var roles = await _userManager.GetRolesAsync(identityUser);
                return Json(new UserDisplayViewModel
                {
                    Id = identityUser.Id,
                    FirstName = identityUser.FirstName ?? "",
                    LastName = identityUser.LastName ?? "",
                    Email = identityUser.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "",
                    ContactNumber = identityUser.PhoneNumber,
                    DateCreated = identityUser.DateCreated,
                    IsActive = identityUser.IsActive,
                    LastLoginDate = identityUser.LastLoginDate,
                    UserType = identityUser.UserType
                });
            }

            return Json(null); // not found
        }

        // POST: NUSAD/Users/DeleteUser/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (int.TryParse(id, out int userId))
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "User deleted successfully." });
                }
            }

            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser != null)
            {
                var result = await _userManager.DeleteAsync(identityUser);
                if (result.Succeeded)
                    return Json(new { success = true, message = "User deleted successfully." });
            }

            return Json(new { success = false, message = "User not found or could not be deleted." });
        }
    }
}
