
using Microsoft.AspNetCore.Identity;
using WMS.DAL.Entities._Identity;
using WMS_DEPI_GRAD.Utilities;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers;

public class AccountController(UserManager<ApplicationUser> _userManager) : Controller 
{
     
    public IActionResult Index()
    {
        return View();
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
    public IActionResult ResetPassword()
    {
        return View();
    }
   // [HttpPost]
    //public IActionResult ResetPassword()
    //{

    //}
}
