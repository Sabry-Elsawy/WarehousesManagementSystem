
using Microsoft.AspNetCore.Identity;
using WMS.DAL.Entities._Identity;
using WMS_DEPI_GRAD.Utilities;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers;

public class AccountController(UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : Controller
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        if (await _userManager.FindByEmailAsync(viewModel.Email) is not { } user)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(viewModel);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, viewModel.Password, true);
        if (result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty, "You are not allowed to login. Please contact support.");
            return View(viewModel);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Your account is locked. Please try again later.");
            return View(viewModel);
        }

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: true);
            return RedirectToAction("Index", "Warehouse");
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password");
        return View(viewModel);
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
            FirstName = viewModel.FirstName,
            LastName = viewModel.LastName,
            PhoneNumber = viewModel.PhoneNumber,
            UserName = viewModel.UserName,
            Email = viewModel.Email,
            Role = UserRole.user
        };
        var Result = await _userManager.CreateAsync(user, viewModel.Password);
        if (Result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            return RedirectToAction("Login");
        }
        else
        {
            foreach (var error in Result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(viewModel);
        }
    }

    //[HttpGet]
    //public IActionResult ForgotPassword()
    //{
    //    return View();
    //}
    //[HttpPost]
    //public IActionResult SendResetPasswordLink(ForgetPasswordViewModel viewModel)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(nameof(ForgotPassword), viewModel);
    //    }

    //    var user = _userManager.FindByEmailAsync(viewModel.Email).Result;
    //        if (user is not null)
    //        {
    //            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
    //            var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = viewModel.Email, token }, Request.Scheme);
    //            var email = new Email()
    //            {
    //                To = viewModel.Email,
    //                Subject = "Password Reset Link",
    //                Body = ResetPasswordLink //ToDo
    //            };
    //            // Send Email    
    //            EmailSettings.SendEmail(email);
    //        }

    //    //  ModelState.AddModelError(string.Empty, "Invalid Operation");
    //    return RedirectToAction(nameof(ResetPassword));
    //   }
    //[HttpGet]
    //public IActionResult ResetPassword(string email, string Token)
    //{
    //    TempData["email"] = email;
    //    TempData["Token"] = Token;

    //    return View();
    //}
    //[HttpPost]
    //public IActionResult ResetPassword(ResetPasswordViewModel ViewModel)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(ViewModel);

    //    }
    //    string email = TempData["email"] as string ?? string.Empty;
    //    string token = TempData["Token"] as string ?? string.Empty;

    //    var user = _userManager.FindByEmailAsync(email).Result;
    //    if (user is not null)
    //    {
    //        var result = _userManager.ResetPasswordAsync(user, token, ViewModel.Password).Result;
    //        if (result.Succeeded)
    //        {
    //            return RedirectToAction(nameof(Index));
    //        }
    //        else
    //        {
    //            foreach (var error in result.Errors)
    //            {
    //                ModelState.AddModelError(string.Empty, error.Description);
    //            }
    //            return View(ViewModel);
    //        }
    //    }
    //    return View(nameof(Index));
    //}

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        var user = await _userManager.FindByEmailAsync(viewModel.Email);
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { email = viewModel.Email, token },
                Request.Scheme);

            var emailMessage = new Email
            {
                To = viewModel.Email,
                Subject = "Password Reset Link",
                Body = $"Click this link to reset your password: {resetLink}"
            };

            EmailSettings.SendEmail(emailMessage);
        }

        // رسالة موحدة لتجنب كشف البريد
        ViewBag.Message = "If your email exists, you will receive a reset link.";
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            return RedirectToAction("Login");

        var model = new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        var user = await _userManager.FindByEmailAsync(viewModel.Email);
        if (user == null)
            return RedirectToAction("Login"); // لا تكشف إذا البريد موجود أم لا

        var result = await _userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);
        if (result.Succeeded)
        {
            ViewBag.Message = "Password has been reset successfully.";
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return RedirectToAction("Login");
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}
