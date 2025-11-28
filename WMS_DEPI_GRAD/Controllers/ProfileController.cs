using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DAL.Entities._Identity;

namespace WMS_DEPI_GRAD.Controllers;
public class ProfileController(UserManager<ApplicationUser> userManager) : Controller
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IActionResult> Index()
    {
        
        var user = await _userManager.Users
                                     .Include(u => u.Address)
                                     .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

        if (user == null)
            return RedirectToAction("Login", "Account"); 

        return View(user);
    }
}
