using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Areas.NUAD.Models;
using MyNursery.Data;
using MyNursery.Services;
using MyNursery.Utility;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ContactController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;

        public ContactController(IEmailSender emailSender, ApplicationDbContext db)
        {
            _emailSender = emailSender;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(ContactMessage model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill out all fields correctly.";
                return RedirectToAction("Index", "Home");
            }

            if (_emailSender is EmailSender emailSenderService)
            {
                var senderValidationResult = await emailSenderService.SendEmailWithValidationAsync(
                    model.Email,
                    "Validation",
                    "Testing"
                );

                if (senderValidationResult != "Success")
                {
                    TempData["Error"] = $"Your email address is invalid: {senderValidationResult}";
                    return RedirectToAction("Index", "Home");
                }

                var adminEmail = ""; // Add your admin email here
                var htmlMessage = $@"
                    <h3>New Contact Message</h3>
                    <p><strong>Name:</strong> {model.Name}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Subject:</strong> {model.Subject}</p>
                    <p><strong>Message:</strong><br />{model.Message}</p>";

                var result = await emailSenderService.SendEmailWithValidationAsync(
                    adminEmail,
                    $"Contact Form: {model.Subject}",
                    htmlMessage
                );

                if (result == "Success")
                {
                    model.SubmittedAt = DateTime.Now;
                    _db.ContactMessages.Add(model);
                    await _db.SaveChangesAsync();

                    TempData["Success"] = "Your message has been sent successfully!";
                }
                else
                {
                    TempData["Error"] = $"Failed to send message to admin: {result}";
                }
            }
            else
            {
                TempData["Error"] = "Email service is not configured correctly.";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ViewMessages()
        {
            var messages = _db.ContactMessages
                .OrderByDescending(m => m.SubmittedAt)
                .ToList();

            return View(messages);
        }

        // New: Return JSON details for a single message
        [HttpGet]
        public IActionResult GetMessageDetails(int id)
        {
            var msg = _db.ContactMessages.FirstOrDefault(m => m.Id == id);
            if (msg == null) return NotFound();

            return Json(new
            {
                name = msg.Name,
                email = msg.Email,
                subject = msg.Subject,
                message = msg.Message,
                submittedAt = msg.SubmittedAt.ToString("yyyy-MM-dd HH:mm")
            });
        }

        // New: Delete a message by id, return JSON result
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMessage(int id)
        {
            var msg = _db.ContactMessages.FirstOrDefault(m => m.Id == id);
            if (msg == null)
                return Json(new { success = false, message = "Message not found." });

            _db.ContactMessages.Remove(msg);
            _db.SaveChanges();

            return Json(new { success = true, message = "Message deleted successfully." });
        }
    }
}
