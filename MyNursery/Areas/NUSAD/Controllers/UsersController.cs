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
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: NUSAD/Users/ManageUsers
        public async Task<IActionResult> ManageUsers()
        {
            // Active registered identity users
            var identityUsersList = await _userManager.Users
                .Where(u => u.IsActive && u.UserType == SD.UserType_Registered)
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

            // Active admin added custom users
            var customUsers = await _context.Users
                .Where(u => u.IsActive && !identityEmails.Contains(u.EmailAddress.ToLower()))
                .Select(u => new UserDisplayViewModel
                {
                    Id = u.Id.ToString(),
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.EmailAddress,
                    Role = u.Role,
                    ContactNumber = u.ContactNumber,
                    DateCreated = u.AddedDate,
                    IsActive = u.IsActive,
                    LastLoginDate = null,
                    UserType = SD.UserType_AdminAdded
                }).ToListAsync();

            var combinedUsers = identityUsersWithRoles.Concat(customUsers).ToList();
            return View(combinedUsers);
        }

        // GET: NUSAD/Users/DisabledUsers
        public async Task<IActionResult> DisabledUsers()
        {
            // Disabled registered identity users
            var disabledIdentityUsers = await _userManager.Users
                .Where(u => !u.IsActive && u.UserType == SD.UserType_Registered)
                .ToListAsync();

            var disabledList = new List<UserDisplayViewModel>();
            foreach (var user in disabledIdentityUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                disabledList.Add(new UserDisplayViewModel
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

            // Disabled admin added custom users
            var disabledCustomUsers = await _context.Users
                .Where(u => !u.IsActive)
                .Select(u => new UserDisplayViewModel
                {
                    Id = u.Id.ToString(),
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.EmailAddress,
                    Role = u.Role,
                    ContactNumber = u.ContactNumber,
                    DateCreated = u.AddedDate,
                    IsActive = u.IsActive,
                    LastLoginDate = null,
                    UserType = SD.UserType_AdminAdded
                }).ToListAsync();

            var result = disabledList.Concat(disabledCustomUsers).ToList();
            return View(result);
        }

        // GET: NUSAD/Users/GetUserDetails/{id}
        [HttpGet]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            // Try parse as int for custom users
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
                        IsActive = u.IsActive,
                        LastLoginDate = null,
                        UserType = SD.UserType_AdminAdded
                    }).FirstOrDefaultAsync();

                if (user != null)
                    return Json(user);
            }

            // Otherwise get from Identity
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

            return Json(null);
        }

        // POST: NUSAD/Users/DisableUser/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableUser(string id)
        {
            if (int.TryParse(id, out int userId))
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.IsActive = false;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "User disabled successfully." });
                }
            }

            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser != null)
            {
                identityUser.IsActive = false;
                var result = await _userManager.UpdateAsync(identityUser);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "User disabled successfully." });
                }
            }

            return Json(new { success = false, message = "User not found or could not be disabled." });
        }

        // POST: NUSAD/Users/EnableUser/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableUser(string id)
        {
            if (int.TryParse(id, out int userId))
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.IsActive = true;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "User enabled successfully." });
                }
            }

            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser != null)
            {
                identityUser.IsActive = true;
                var result = await _userManager.UpdateAsync(identityUser);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "User enabled successfully." });
                }
            }

            return Json(new { success = false, message = "User not found or could not be enabled." });
        }
    }
}
