using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.DAL.Constants;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers
{
    [Authorize(Roles = "Admin")] // Restrict to Admin only
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // Read-only: Display list of roles
        public async Task<IActionResult> Index()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.Message = "Role management is now done through User Management. This page is read-only.";
            return View(roles);
        }

        // All CRUD operations removed - use User Management page to assign roles to users
        // Create, Edit, Delete, and ManagePermissions actions have been removed
        // Role assignment is handled in UserController.Edit
    }
}
