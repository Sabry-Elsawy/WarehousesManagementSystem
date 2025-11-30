using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.DAL.Constants;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers
{
    // [Authorize(Roles = "Admin")] // Uncomment to enforce Admin only access
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (await _roleManager.RoleExistsAsync(role.Name))
            {
                ModelState.AddModelError("Name", "Role already exists");
                return View(role);
            }

            await _roleManager.CreateAsync(role);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IdentityRole role)
        {
            var existingRole = await _roleManager.FindByIdAsync(role.Id);
            if (existingRole == null) return NotFound();

            existingRole.Name = role.Name;
            // existingRole.NormalizedName = role.Name.ToUpper(); // Managed by UpdateAsync usually

            var result = await _roleManager.UpdateAsync(existingRole);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(role);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManagePermissions(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            var model = new PermissionViewModel
            {
                RoleId = roleId,
                RoleName = role.Name,
                RoleClaims = new List<RoleClaimsViewModel>()
            };

            var allPermissions = Permissions.GetAllPermissions();
            var existingClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var permission in allPermissions)
            {
                var roleClaim = new RoleClaimsViewModel
                {
                    Type = "Permission",
                    Value = permission
                };

                if (existingClaims.Any(c => c.Value == permission))
                {
                    roleClaim.Selected = true;
                }

                model.RoleClaims.Add(roleClaim);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManagePermissions(PermissionViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null) return NotFound();

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            
            // Remove all existing permission claims
            foreach (var claim in existingClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // Add selected claims
            var selectedClaims = model.RoleClaims.Where(c => c.Selected).ToList();
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", claim.Value));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
