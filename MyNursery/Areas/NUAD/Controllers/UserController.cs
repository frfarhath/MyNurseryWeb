using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Areas.NUAD.Models;
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
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Helper: get roles excluding system/admin roles
        private List<string> GetAvailableRoles()
        {
            var excludedRoles = new[]
            {
                SD.Role_Admin,
                SD.Role_SuperAdmin,
                SD.Role_OtherUser,
                SD.Role_User,
                SD.Role_AdminCSAD
            };

            return _roleManager.Roles
                .Select(r => r.Name)
                .Where(r => !excludedRoles.Contains(r))
                .ToList();
        }

        // Redirect CreateUser GET to Upsert (create mode)
        [HttpGet]
        public IActionResult CreateUser()
        {
            return RedirectToAction("Upsert");
        }

        // Redirect EditUser GET to Upsert (edit mode)
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            return RedirectToAction("Upsert", new { id });
        }

        // Redirect EditUser POST to Upsert POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(User model)
        {
            return await Upsert(model);
        }

        // List users
        public IActionResult ManageUsers()
        {
            var users = _context.Users.OrderByDescending(u => u.AddedDate).ToList();
            return View(users);
        }

        // GET: Upsert (create or edit form)
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ViewBag.Roles = GetAvailableRoles();

            if (id == null || id == 0)
            {
                // Create mode
                return View(new User());
            }

            // Edit mode
            var user = _context.Users.Find(id);
            if (user == null)
            {
                TempData[SD.Error_Msg] = "User not found.";
                return RedirectToAction(nameof(ManageUsers));
            }

            return View(user);
        }

        // POST: Upsert (create or edit user)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(User model)
        {
            ViewBag.Roles = GetAvailableRoles();

            if (!ModelState.IsValid)
            {
                TempData[SD.Error_Msg] = "Validation failed. Please try again.";
                return View(model);
            }

            if (model.Id == 0)
            {
                // Create new Identity user
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
                    UserType = SD.UserType_AdminAdded,
                    Area = "NUUS",
                    MustChangePassword = true
                };

                var createResult = await _userManager.CreateAsync(identityUser, generatedPassword);
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    TempData[SD.Error_Msg] = "Failed to create user in Identity.";
                    return View(model);
                }

                // Assign roles
                var roleResult = await _userManager.AddToRoleAsync(identityUser, model.Role);
                var baseRoleResult = await _userManager.AddToRoleAsync(identityUser, SD.Role_OtherUser);

                if (!roleResult.Succeeded || !baseRoleResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to assign user roles.");
                    TempData[SD.Error_Msg] = "Failed to assign roles.";
                    return View(model);
                }

                // Add to custom Users table
                model.Password = null; // never store plain password
                model.AddedDate = DateTime.UtcNow;
                model.Area = "NUUS";
                model.IsActive = true;

                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                // Send welcome email with temp password
                string emailBody = $@"
                    Dear {model.FirstName},<br/><br/>
                    You have been successfully added to <strong>Little Sprouts Nursery</strong> as a <strong>{model.Role}</strong>.<br/><br/>
                    <strong>Temporary Password:</strong><br/>
                    <code>{generatedPassword}</code><br/><br/>
                    Please log in and change your password immediately.<br/><br/>
                    Best regards,<br/>
                    <strong>Little Sprouts Nursery Team</strong>";

                try
                {
                    await _emailSender.SendEmailAsync(model.EmailAddress, "Welcome to MyNursery", emailBody);
                }
                catch
                {
                    TempData[SD.Warning_Msg] = "User created but email could not be sent.";
                }

                TempData[SD.Success_Msg] = "User created and credentials emailed!";
            }
            else
            {
                // Edit existing user
                var userInDb = _context.Users.Find(model.Id);
                if (userInDb == null)
                {
                    TempData[SD.Error_Msg] = "User not found.";
                    return RedirectToAction(nameof(ManageUsers));
                }

                // Update fields
                userInDb.FirstName = model.FirstName;
                userInDb.LastName = model.LastName;
                userInDb.EmailAddress = model.EmailAddress;
                userInDb.Role = model.Role;
                userInDb.ContactNumber = model.ContactNumber;
                userInDb.IsActive = model.IsActive;

                _context.Users.Update(userInDb);
                await _context.SaveChangesAsync();

                TempData[SD.Success_Msg] = "User updated successfully!";
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        // Delete user POST
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var customUser = await _context.Users.FindAsync(id);
            if (customUser == null)
                return Json(new { success = false, message = "User not found." });

            var identityUser = await _userManager.FindByEmailAsync(customUser.EmailAddress);
            if (identityUser != null)
            {
                var deleteResult = await _userManager.DeleteAsync(identityUser);
                if (!deleteResult.Succeeded)
                    return Json(new { success = false, message = "Failed to delete user from Identity." });
            }

            _context.Users.Remove(customUser);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "User deleted successfully from both tables." });
        }

        // Fetch user details for AJAX
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
                user.Area,
                AddedDate = user.AddedDate.ToString("yyyy-MM-dd HH:mm")
            });
        }

        // Password generator helper
        private string GenerateRandomPassword(int length = 10)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&*?";
            Random rnd = new();
            return new string(Enumerable.Repeat(valid, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
