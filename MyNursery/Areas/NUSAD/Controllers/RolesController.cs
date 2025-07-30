using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Areas.NUSAD.Models;
using MyNursery.Utility;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUSAD.Controllers
{
    [Area("NUSAD")]
    [Authorize(Roles = SD.Role_SuperAdmin)]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult ManageRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles); // View expects: @model IEnumerable<ApplicationRole>
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData[SD.Error_Msg] = "Role name cannot be empty.";
                return RedirectToAction(nameof(ManageRoles));
            }

            if (await _roleManager.RoleExistsAsync(name.Trim()))
            {
                TempData[SD.Error_Msg] = "Role already exists.";
                return RedirectToAction(nameof(ManageRoles));
            }

            var newRole = new ApplicationRole
            {
                Name = name.Trim(),
                NormalizedName = name.Trim().ToUpper(),
                Description = description?.Trim()
            };

            var result = await _roleManager.CreateAsync(newRole);

            if (result.Succeeded)
            {
                TempData[SD.Success_Msg] = "Role added successfully.";
            }
            else
            {
                TempData[SD.Error_Msg] = string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(ManageRoles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, string name, string? description)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData[SD.Error_Msg] = "Role not found.";
                return RedirectToAction(nameof(ManageRoles));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData[SD.Error_Msg] = "Role name cannot be empty.";
                return RedirectToAction(nameof(ManageRoles));
            }

            role.Name = name.Trim();
            role.NormalizedName = name.Trim().ToUpper();
            role.Description = description?.Trim();

            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData[SD.Success_Msg] = "Role updated successfully.";
            }
            else
            {
                TempData[SD.Error_Msg] = string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(ManageRoles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData[SD.Error_Msg] = "Role not found.";
                return RedirectToAction(nameof(ManageRoles));
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                TempData[SD.Success_Msg] = "Role deleted successfully.";
            }
            else
            {
                TempData[SD.Error_Msg] = string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(ManageRoles));
        }
    }
}
