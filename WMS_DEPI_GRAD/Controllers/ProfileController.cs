using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DAL.Entities._Identity;
using WMS_DEPI_GRAD.ViewModels;

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

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.Users
                                     .Include(u => u.Address)
                                     .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

        if (user == null)
            return RedirectToAction("Login", "Account");

        var model = new ProfileViewModel
        {
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            UserName = user.UserName ?? string.Empty,
            AddressStreet = user.Address?.Street,
            AddressCity = user.Address?.City,
            AddressState = user.Address?.State,
            AddressPostalCode = user.Address?.PostalCode,
            AddressCountry = user.Address?.Country,
            Phone = user.Address?.Phone,
            Company = user.Address?.Company
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.Users
                                     .Include(u => u.Address)
                                     .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

        if (user == null)
            return RedirectToAction("Login", "Account");

        // Update user basic info
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;

        // Update or create address
        if (user.Address == null)
        {
            user.Address = new Address
            {
                UserId = user.Id,
                Street = model.AddressStreet ?? string.Empty,
                City = model.AddressCity ?? string.Empty,
                State = model.AddressState,
                PostalCode = model.AddressPostalCode,
                Country = model.AddressCountry ?? string.Empty,
                Phone = model.Phone,
                Company = model.Company
            };
        }
        else
        {
            user.Address.Street = model.AddressStreet ?? string.Empty;
            user.Address.City = model.AddressCity ?? string.Empty;
            user.Address.State = model.AddressState;
            user.Address.PostalCode = model.AddressPostalCode;
            user.Address.Country = model.AddressCountry ?? string.Empty;
            user.Address.Phone = model.Phone;
            user.Address.Company = model.Company;
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }
}
