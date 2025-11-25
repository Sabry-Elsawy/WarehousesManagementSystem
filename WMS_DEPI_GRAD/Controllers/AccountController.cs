
using Microsoft.AspNetCore.Identity;
using WMS.DAL.Entities._Identity;
using WMS_DEPI_GRAD.Utilities;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers;

public class AccountController(UserManager<ApplicationUser> _userManager) : Controller 
{
     
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        var user = new ApplicationUser()
        {
            PhoneNumber = viewModel.PhoneNumber,
            UserName = viewModel.UserName,
            Email = viewModel.Email,
            Role = UserRole.user
        };
        var Result = await _userManager.CreateAsync(user, viewModel.Password);
        if (Result.Succeeded)
            return RedirectToAction("Login");
        else
        {
            foreach (var error in Result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(viewModel);
        }
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }
    [HttpPost]
    public IActionResult SendResetPasswordLink(ForgetPasswordViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
         
        var user = _userManager.FindByEmailAsync(viewModel.Email).Result;
        if (user is not null)
        {
                var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                var ResetPasswordLink = Url.Action("ResetPassword", "Account",new { email = viewModel.Email, token },Request.Scheme);
                var email = new Email()
                {
                    To = viewModel.Email,
                    Subject = "Password Reset Link",
                    Body = ResetPasswordLink //ToDo
                };
                // Send Email    
                EmailSettings.SendEmail(email);
            }
         
        }
        ModelState.AddModelError(string.Empty, "Invalid Operation");
        return View(nameof(ForgotPassword), viewModel);
    }
    [HttpGet]
    public IActionResult ResetPassword(string email , string Token)
    {
        TempData["email"] = email;
        TempData["Token"] = Token;

        return View();
    }
    [HttpPost]
    public IActionResult ResetPassword(ResetPasswordViewModel ViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(ViewModel);

        }
        string email = TempData["email"] as string?? string.Empty;
        string token = TempData["Token"] as string ?? string.Empty;

        var user = _userManager.FindByEmailAsync(email).Result;
        if (user is not null)
        {
            var result = _userManager.ResetPasswordAsync(user, token, ViewModel.Password).Result;
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(ViewModel);
            }
        }
        return View(nameof(Index)); 
    }
}
