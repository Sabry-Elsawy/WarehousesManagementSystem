
using Microsoft.AspNetCore.Identity;
using WMS.DAL.Entities._Identity;
using WMS_DEPI_GRAD.Utilities;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers;

public class AccountController(UserManager<ApplicationUser> _userManager,
    SignInManager<ApplicationUser> _signInManager) : Controller
{
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

            return RedirectToAction("Index", "Inventory");
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
            PhoneNumber = viewModel.PhoneNumber,
            UserName = viewModel.UserName,
            Email = viewModel.Email,
            Role = UserRole.user
        };
        var Result = await _userManager.CreateAsync(user, viewModel.Password);
        if (Result.Succeeded)
        {
            //await _userManager.AddToRoleAsync(user, "User");
            //return RedirectToAction("Login");

            await _userManager.AddToRoleAsync(user, "User");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = user.Id, token = token },
                protocol: HttpContext.Request.Scheme);

            var email = new Email()
            {
                To = user.Email,
                Subject = "Confirm your account",
                Body = $@"
        <h3>Welcome {user.UserName}</h3>
        <p>Please confirm your email by clicking the link below:</p>
        <p><a href='{confirmationLink}'>Click here to confirm your account</a></p>
    "
            };


            EmailSettings.SendEmail(email);

            return RedirectToAction("EmailSent"); 
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
                var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = viewModel.Email, token }, Request.Scheme);
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
    public IActionResult ResetPassword(string email, string Token)
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
        string email = TempData["email"] as string ?? string.Empty;
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

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult EmailSent()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return BadRequest("User ID and Token are required.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
            return View("ConfirmEmailSuccess"); 
        else
            return View("Error"); 
    }

}
