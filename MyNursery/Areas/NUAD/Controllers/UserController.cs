using Microsoft.AspNetCore.Mvc;
using MyNursery.Models;
using MyNursery.Data;
using MyNursery.Services;
using System.Linq;
using System.Threading.Tasks;
using MyNursery.Areas.NUAD.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using MyNursery.Utility;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public UsersController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public IActionResult ManageUsers()
        {
            var users = _context.Users.OrderByDescending(u => u.AddedDate).ToList();
            return View(users);
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                user.AddedDate = DateTime.UtcNow;

                // 🔐 Generate random password
                string generatedPassword = GenerateRandomPassword();

                // 🚫 Not storing password in DB — just emailing it

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // ✉️ Send welcome email with role and password
                string message = $@"
    Dear {user.FirstName},<br/><br/>

    You have been successfully added to <strong>Little Sprouts Nursery</strong> as a <strong>{user.Role}</strong>.<br/><br/>

    <strong>Temporary Password:</strong><br/>
    <code>{generatedPassword}</code><br/><br/>

    Please keep this password secure and confidential. We recommend changing it as soon as possible.<br/><br/>

    If you have any questions or need assistance, feel free to contact the administrator.<br/><br/>

    Best regards,<br/>
    <strong>Little Sprouts Nursery Team</strong>
";


                await _emailSender.SendEmailAsync(user.EmailAddress, "Welcome to MyNursery", message);

                TempData[SD.Success_Msg] = "User created and password emailed successfully!";
                return RedirectToAction("ManageUsers");
            }

            TempData[SD.Error_Msg] = "Please correct the errors and try again.";
            return View(user);
        }

        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                TempData[SD.Error_Msg] = "User not found.";
                return RedirectToAction("ManageUsers");
            }

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

        // 🔐 Password generator
        private string GenerateRandomPassword(int length = 10)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&*?";
            Random rnd = new();
            return new string(Enumerable.Repeat(valid, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
