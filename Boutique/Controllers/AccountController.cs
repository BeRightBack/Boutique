using System.Security.Claims;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Boutique.Services;
using Boutique.Models.AccountViewModels;
using Boutique.Entity;

namespace Boutique.Controllers;
public class AccountController : BaseController
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;

    [TempData]
    public string ErrorMessage { get; set; }

    public AccountController(ILanguageService languageService,
     ILocalizationService localizationService,
     SignInManager<ApplicationUser> signInManager,
     UserManager<ApplicationUser> userManager,
     IEmailSender emailSender) : base(languageService, localizationService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _signInManager.SignOutAsync();

        var _user = _userManager.Users.FirstOrDefault(u => u.UserName == model.UserName);

        if (_user == null)
        {
            _user = _userManager.Users.FirstOrDefault(u => u.Email == model.UserName);
            if(_user == null) {
                ModelState.AddModelError(string.Empty, LocString("Invalid login attempt."));
                return View(model);
            }
        }

        ApplicationUser user = await _userManager.FindByEmailAsync(_user.Email);

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                $"Please confirm your account by clicking this link: {callbackUrl}");
            ModelState.AddModelError(string.Empty, "You must have a confirmed email to log in.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(_user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            Log.Information("User logged in.");

            if (Url.IsLocalUrl(returnUrl))
            {
                return TrimAndTransformReturnUrl(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        else if (result.RequiresTwoFactor)
        {
            return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = returnUrl, model.RememberMe });
        }
        else if (result.IsLockedOut)
        {
            return View("Lockout");
        }
        else
        {
            ModelState.AddModelError(string.Empty, LocString("Invalid login attempt."));
            return View(model);
        }
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        _ = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");

        var model = new LoginWith2faViewModel { RememberMe = rememberMe };
        ViewData["ReturnUrl"] = returnUrl;
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _ = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");

        var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

        if (result.Succeeded)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        if (result.IsLockedOut)
        {
            return View("Lockout");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    $"Please confirm your account by clicking this link: {callbackUrl} ");
                //await _signInManager.SignInAsync(user, isPersistent: false);
                Log.Information("User created a new account with password.");
                ViewData["Message"] = "Check your email and confirm your account, you must be confirmed before you can log in.";
                return View("RegisterConfirmation");
            }
            AddErrors(result);
        }

        // If we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegisterConfirmation()
    {

        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        _ = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _ = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");

        var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty).Replace("-", string.Empty);
        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
        if (result.Succeeded)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        if (result.IsLockedOut)
        {
            return View("Lockout");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
            return View(model);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Lockout()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        // Request a redirect to the external login provider
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToAction(nameof(Login));
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }
        if (result.IsLockedOut)
        {
            return View("Lockout");
        }
        else
        {
            // If the user does not have an account, then ask the user to create an account.
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = info.LoginProvider;
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("ExternalLoginFailure");
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }
            }
            AddErrors(result); // If we got this far, something failed, redisplay form
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        var result = await _userManager.ConfirmEmailAsync(user, code);
        return View(result.Succeeded ? "ConfirmEmail" : "Error");
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendEmailConfirmation()
    {
        var user = await _userManager.FindByEmailAsync(User.Identity.Name);
        if (user == null)
        {
            return RedirectToAction(nameof(Login));
        }
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
        await _emailSender.SendEmailAsync(User.Identity.Name, "Confirm your email",
            $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
        ViewData["Message"] = "Check your email and confirm your account, you must be confirmed before you can log in.";
        return RedirectToAction(nameof(RegisterConfirmation));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                $"Please reset your password by clicking here: {callbackUrl}");
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string code = null)
    {
        return code == null ? View("Error") : View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }
        var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }
        AddErrors(result);
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    #region Helpers
    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    // private IActionResult RedirectToLocal(string returnUrl)
    // {
    //     if (Url.IsLocalUrl(returnUrl))
    //     {
    //         return Redirect(returnUrl);
    //     }
    //     else
    //     {
    //         return RedirectToAction(nameof(HomeController.Index), "Home");
    //     }
    // }

    // public ActionResult RedirectToLocal(string returnUrl)
    // {
    //     Log.Information(returnUrl);
    //     if (Url.IsLocalUrl(returnUrl))
    //     {
    //         returnUrl = returnUrl.Trim('/'); // trim leading and trailing slashes
    //         if (returnUrl.StartsWith("~/"))
    //         {
    //             Log.Information(returnUrl.Substring(2));
    //             // transform relative URL to a route
    //             return RedirectToAction(returnUrl.Substring(2));
    //         }
    //         else
    //         {
    //             Log.Information(returnUrl);
    //             // assume absolute URL or route name
    //             return Redirect(returnUrl);
    //         }
    //     }
    //     else
    //     {
    //         // return default action or route
    //         return RedirectToAction("Index", "Home");
    //     }
    // }

    // public IActionResult TrimAndTransformReturnUrl(string returnUrl)
    // {
    //     // Trim any leading or trailing whitespace from the returnUrl
    //     returnUrl = returnUrl.Trim();

    //     // Split the returnUrl into its components
    //     string[] returnUrlParts = returnUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

    //     // Determine the controller, action, and parameters from the returnUrl parts
    //     string controller = returnUrlParts.Length > 0 ? returnUrlParts[0] : "Home";
    //     string action = returnUrlParts.Length > 1 ? returnUrlParts[1] : "Index";
    //     object routeValues = null;

    //     if (returnUrlParts.Length > 2)
    //     {
    //         // If there are parameters, we need to create a dictionary to store them
    //         Dictionary<string, string> parameters = new Dictionary<string, string>();

    //         // Loop through the remaining parts of the returnUrl and add them to the dictionary
    //         for (int i = 2; i < returnUrlParts.Length; i++)
    //         {
    //             string[] parameterParts = returnUrlParts[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

    //             // Make sure we have a key-value pair
    //             if (parameterParts.Length == 2)
    //             {
    //                 parameters[parameterParts[0]] = parameterParts[1];
    //             }
    //         }

    //         routeValues = parameters;
    //     }

    //     Log.Information(action);
    //     Log.Information(controller);

    //     // Return a RedirectToAction with the appropriate controller, action, and parameters
    //     return RedirectToAction(action, controller, routeValues);
    // }

    public IActionResult TrimAndTransformReturnUrl(string returnUrl)
    {
        // Trim any leading or trailing whitespace from the returnUrl
        returnUrl = returnUrl.Trim();

        // Split the returnUrl into its components
        string[] returnUrlParts = returnUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        // Determine the controller and action from the returnUrl parts
        string controller = returnUrlParts.ElementAtOrDefault(0) ?? "Home";
        string action = returnUrlParts.ElementAtOrDefault(1) ?? "Index";

        // If there are parameters, use LINQ to create a dictionary
        var parameters = returnUrlParts.Skip(2)
                                       .Select(part => part.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                                       .Where(parts => parts.Length == 2)
                                       .ToDictionary(parts => parts[0], parts => parts[1]);

        Log.Information(action);
        Log.Information(controller);

        // Return a RedirectToAction with the appropriate controller, action, and parameters
        return RedirectToAction(action, controller, parameters);
    }


    #endregion
}