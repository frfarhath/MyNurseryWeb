using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Areas.NUSAD.Models;
using MyNursery.Areas.Welcome.Models;
using MyNursery.Data;
using MyNursery.Models;
using MyNursery.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;  // Changed here

        public UsersController(ApplicationDbContext context, IEmailSender emailSender,
            UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) // Changed here
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private List<string> GetAvailableRoles()
        {
            var excludedRoles = new[] { SD.Role_Admin,
    SD.Role_SuperAdmin,
    SD.Role_OtherUser,
    SD.Role_User,
    SD.Role_AdminCSAD };

            return _roleManager.Roles
                .Select(r => r.Name)
                .Where(r => !excludedRoles.Contains(r))
                .ToList();
        }

        public IActionResult ManageUsers()
        {
            var users = _context.Users.OrderByDescending(u => u.AddedDate).ToList();
            return View(users);
        }

        public IActionResult CreateUser()
        {
            ViewBag.Roles = GetAvailableRoles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = GetAvailableRoles();
                return View(model);
            }

            string generatedPassword = GenerateRandomPassword();

            var identityUser = new ApplicationUser
            {
                UserName = model.EmailAddress,
                Email = model.EmailAddress,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.ContactNumber,
                EmailConfirmed = true,
                DateCreated = DateTime.UtcNow,
                UserType = SD.UserType_AdminAdded
            };

            var result = await _userManager.CreateAsync(identityUser, generatedPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                ViewBag.Roles = GetAvailableRoles();
                return View(model);
            }

            await _userManager.AddToRoleAsync(identityUser, model.Role);

            model.Password = null;
            model.AddedDate = DateTime.UtcNow;
            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            string message = $@"
                Dear {model.FirstName},<br/><br/>
                You have been successfully added to <strong>Little Sprouts Nursery</strong> as a <strong>{model.Role}</strong>.<br/><br/>
                <strong>Temporary Password:</strong><br/>
                <code>{generatedPassword}</code><br/><br/>
                Please log in and change your password.<br/><br/>
                Best regards,<br/>
                <strong>Little Sprouts Nursery Team</strong>";

            await _emailSender.SendEmailAsync(model.EmailAddress, "Welcome to MyNursery", message);

            TempData[SD.Success_Msg] = "User created and credentials emailed!";
            return RedirectToAction("ManageUsers");
        }

        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                TempData[SD.Error_Msg] = "User not found.";
                return RedirectToAction("ManageUsers");
            }

            ViewBag.Roles = GetAvailableRoles();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData[SD.Success_Msg] = "User updated successfully!";
                return RedirectToAction("ManageUsers");
            }

            ViewBag.Roles = GetAvailableRoles();
            TempData[SD.Error_Msg] = "Validation failed. Please try again.";
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "User deleted successfully." });
        }

        public IActionResult GetUserDetails(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            return Json(new
            {
                user.FirstName,
                user.LastName,
                user.EmailAddress,
                user.Role,
                user.ContactNumber,
                AddedDate = user.AddedDate.ToString("yyyy-MM-dd HH:mm")
            });
        }

        private string GenerateRandomPassword(int length = 10)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&*?";
            Random rnd = new();
            return new string(Enumerable.Repeat(valid, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
