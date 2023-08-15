using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Boutique.Entity;
using Boutique.Models.ManageViewModels;
using Boutique.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileProviders;
using Serilog;

namespace Boutique.Controllers;

public class ManageController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly UrlEncoder _urlEncoder;

    public ManageController(ILanguageService languageService,
         ILocalizationService localizationService,
         SignInManager<ApplicationUser> signInManager,
         UserManager<ApplicationUser> userManager,
         IEmailSender emailSender,
         UrlEncoder urlEncoder) : base(languageService, localizationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _urlEncoder = urlEncoder;
    }

    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    [TempData]
    public string ErrorMessage { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [TempData]
    public string UserNameChangeLimitMessage { get; set; }

    public string AuthenticatorUri { get; set; }

    public string SharedKey { get; set; }

    public string[] RecoveryCodes { get; set; }

    public string Username { get; set; }
    public string RecoveryCodesKey { get; set; }



    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // var returnUrl = Request.Query["returnUrl"].ToString();
        // ReturnUrlObj.ReturnUrl = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/";

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }
        UserNameChangeLimitMessage = $"You can change your username {user.UsernameChangeLimit} more time(s).";

        var userName = await _userManager.GetUserNameAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        var firstName = user.FirstName;
        var lastName = user.LastName;
        var profilePicture = user.ProfilePicture;

        var model = new IndexViewModel
        {
            PhoneNumber = phoneNumber,
            Username = userName,
            FirstName = firstName,
            LastName = lastName,
            ProfilePicture = profilePicture,
            ProfilePicturePath = user.ProfilePicturePath,
            StatusMessage = StatusMessage,
            UserNameChangeLimitMessage = UserNameChangeLimitMessage
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IndexViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            return View("Profile", model);
        }

        var firstName = user.FirstName;
        var lastName = user.LastName;
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        if (model.FirstName != firstName)
        {
            user.FirstName = model.FirstName;
            await _userManager.UpdateAsync(user);

            var existingClaims = _userManager.GetClaimsAsync(user).Result;
            var _claim = existingClaims.FirstOrDefault(x => x.Type == "user.firstname");
            if (_claim != null)
                await _userManager.RemoveClaimAsync(user, _claim);

            // add additional claims for this new user..

            await _userManager.AddClaimsAsync(user, new Claim[]
            {
                            new Claim("user.firstname", model.FirstName)
            });
        }

        if (model.LastName != lastName)
        {
            user.LastName = model.LastName;
            await _userManager.UpdateAsync(user);

            var existingClaims = _userManager.GetClaimsAsync(user).Result;
            var _claim = existingClaims.FirstOrDefault(x => x.Type == "user.lastname");
            if (_claim != null)
                await _userManager.RemoveClaimAsync(user, _claim);

            // add additional claims for this new user..

            await _userManager.AddClaimsAsync(user, new Claim[]
            {
                            new Claim("user.lastname", model.LastName)
            });
        }

        if (model.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return View("Profile", model);
            }
        }
        if (user.UsernameChangeLimit > 0)
        {
            if (model.Username != user.UserName)
            {
                var userNameExists = await _userManager.FindByNameAsync(model.Username);
                if (userNameExists != null)
                {
                    StatusMessage = "User name already taken. Select a different username.";
                    return View("Profile", model);
                }

                var setUserName = await _userManager.SetUserNameAsync(user, model.Username);
                if (!setUserName.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set user name.";
                    return View("Profile", model);
                }
                else
                {
                    user.UsernameChangeLimit -= 1;
                    await _userManager.UpdateAsync(user);

                    var existingClaims = _userManager.GetClaimsAsync(user).Result;
                    var _claim = existingClaims.FirstOrDefault(x => x.Type == "user.username");
                    if (_claim != null)
                        await _userManager.RemoveClaimAsync(user, _claim);

                    // add additional claims for this new user..

                    await _userManager.AddClaimsAsync(user, new Claim[]
                    {
                            new Claim("user.username", model.Username)


                });
                }
            }
        }
        if (model.ProfilePictureFile != null)
        {
            var file = model.ProfilePictureFile;

            //Getting FileName
            var fileName = Path.GetFileName(file.FileName);

            //Assigning Unique Filename (Guid)
            var myUniqueFileName = Convert.ToString(Guid.NewGuid());

            //Getting file Extension
            var fileExtension = Path.GetExtension(fileName);

            // concatenating  FileName + FileExtension
            var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

            var subPath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images")).Root + $@"/{user.Id}";

            Directory.CreateDirectory(subPath);

            // Combines two strings into a path.
            var filepath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images")).Root + $@"/{user.Id}/{newFileName}";

            using (FileStream fs = System.IO.File.Create(filepath))
            {
                await file.CopyToAsync(fs);
                user.ProfilePicturePath = "/Images/" + $"{user.Id}/{newFileName}";
                fs.Flush();
            }

            using (var dataStream = new MemoryStream())
            {
                await file.CopyToAsync(dataStream);
                user.ProfilePicture = dataStream.ToArray();
            }
            await _userManager.UpdateAsync(user);

            var existingClaims = _userManager.GetClaimsAsync(user).Result;
            var _claim = existingClaims.FirstOrDefault(x => x.Type == "user.picture");
            if (_claim != null)
                await _userManager.RemoveClaimAsync(user, _claim);

            // add additional claims for this new user..

            await _userManager.AddClaimsAsync(user, new Claim[]
            {
                        new Claim("user.picture", "/Images/" + $"{user.Id}/{newFileName}")
            });

        }
        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        //return RedirectToAction("Profile", "Manage");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Email(EmailViewModel model = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var email = await _userManager.GetEmailAsync(user);

        model ??= new EmailViewModel
        {
            Email = email,
            IsEmailConfirmed = user.EmailConfirmed,
            StatusMessage = StatusMessage
        };

        return View(model);
    }


    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return RedirectToAction(nameof(SetPassword));
        }

        var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your password has been changed.";

        return RedirectToAction(nameof(ChangePassword));
    }

    [HttpGet]
    public async Task<IActionResult> SetPassword()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);

        if (hasPassword)
        {
            return RedirectToAction(nameof(ChangePassword));
        }

        var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            foreach (var error in addPasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your password has been set.";

        return RedirectToAction(nameof(SetPassword));

    }

    [HttpGet]
    public async Task<IActionResult> ExternalLogins()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
        model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
        .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
        .ToList();
        model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
        model.StatusMessage = StatusMessage;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkLogin(string provider)
    {
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        // Request a redirect to the external login provider to link a login for the current user
        var redirectUrl = Url.Action(nameof(LinkLoginCallback));
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
        return new ChallengeResult(provider, properties);
    }

    [HttpGet]
    public async Task<IActionResult> LinkLoginCallback()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync(user.Id) ?? throw new InvalidOperationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");

        var result = await _userManager.AddLoginAsync(user, info);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        StatusMessage = "The external login was added.";
        return RedirectToAction(nameof(ExternalLogins));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "The external login was removed.";
        return RedirectToAction(nameof(ExternalLogins));
    }

    [HttpGet]
    public async Task<IActionResult> TwoFactorAuthentication()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var model = new TwoFactorAuthenticationViewModel
        {
            HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
            Is2faEnabled = user.TwoFactorEnabled,
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Disable2fa()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error when disabling 2FA for user with ID '{user.Id}'.");
        }

        ViewData["StatusMessage"] = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpGet]
    public async Task<IActionResult> Enable2fa()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var enable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, true);
        if (!enable2faResult.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error when enabling 2FA for user with ID '{user.Id}'.");
        }

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpGet]
    public async Task<IActionResult> ResetAuthenticator()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);
        await _userManager.UpdateAsync(user);
        StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpGet]
    public async Task<IActionResult> GenerateRecoveryCodes()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        TempData["RecoveryCodes"] = recoveryCodes.ToArray();

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpGet]
    public async Task<IActionResult> RemoveRecoveryCodes()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        TempData["RecoveryCodes"] = recoveryCodes.ToArray();

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpGet]
    public async Task<IActionResult> Disable()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await _userManager.SetLockoutEnabledAsync(user, false);
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(AccountController.Lockout));
    }

    [HttpGet]
    public async Task<IActionResult> Enable()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await _userManager.SetLockoutEnabledAsync(user, true);
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(AccountController.Lockout));
    }

    [HttpGet]
    public async Task<IActionResult> PersonalData()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var personalData = new PersonalDataViewModel
        {
            UserId = await _userManager.GetUserIdAsync(user),
            Email = await _userManager.GetEmailAsync(user),
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
            PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
            IsPhoneNumberConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user),
            TwoFactor = await _userManager.GetTwoFactorEnabledAsync(user),
            Logins = await _userManager.GetLoginsAsync(user),
            ExternalLogins = await _userManager.GetLoginsAsync(user),
            // Other personal data for your application can be retrieved here as well,
            // including name, birthday, address, etc.
        };

        return View(personalData);
    }

    [HttpPost]
    public async Task<IActionResult> PersonalData(PersonalDataViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userManager.SetEmailAsync(user, model.Email);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error when setting email for user with ID '{user.Id}'.");
        }

        result = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error when setting phone number for user with ID '{user.Id}'.");
        }

        await _userManager.UpdateAsync(user);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> DeletePersonalData()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var model = new DeletePersonalDataViewModel
        {
            RequirePassword = await _userManager.HasPasswordAsync(user)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeletePersonalData(DeletePersonalDataViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var result = await _userManager.DeleteAsync(user);
        var userId = await _userManager.GetUserIdAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error when deleting user with ID '{userId}'.");
        }

        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DownloadPersonalData()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        // Only include personal data for download
        var personalDataProps = typeof(PersonalDataViewModel).GetProperties().Select(p => p.Name);
        var personalData = new Dictionary<string, string>();
        foreach (var prop in personalDataProps)
        {
            personalData.Add(prop, typeof(ApplicationUser).GetProperty(prop).GetValue(user)?.ToString() ?? "null");
        }

        var logins = await _userManager.GetLoginsAsync(user);
        foreach (var login in logins)
        {
            personalData.Add($"{login.LoginProvider} external login provider key", login.ProviderKey);
        }

        personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

        Response.Headers.Append("Content-Disposition", "attachment; filename=PersonalData.json");
        return new FileContentResult(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
    }

    [HttpPost]
    public async Task<IActionResult> ChangeEmailAsync(EmailViewModel model = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            return View("Email");
        }

        var email = await _userManager.GetEmailAsync(user);
        if (model.NewEmail != email)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmailChange",
                pageHandler: null,
                values: new { area = "", userId, email = model.NewEmail, code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                model.NewEmail,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Confirmation link to change email sent. Please check your email.";
            return View("Email");
        }

        StatusMessage = "Your email is unchanged.";
        return View("Email");
    }

    [HttpPost]
    public async Task<IActionResult> SendVerificationEmailAsync()
    {

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            return View("Email");
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var email = await _userManager.GetEmailAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "", userId, code },
            protocol: Request.Scheme);
        await _emailSender.SendEmailAsync(
            email,
            "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        StatusMessage = "Verification email sent. Please check your email.";
        return View("Email");
    }
    // private async Task LoadAsync(ApplicationUser user, EmailViewModel model=null)
    // {
    //     var email = await userManager.GetEmailAsync(user);
    //     model.Email = email;
    //     model.NewEmail = email;
    //     model.IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
    // }

    [HttpGet]
    public async Task<IActionResult> EnableAuthenticator()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await LoadSharedKeyAndQrCodeUriAsync(user);

        return View("EnableAuthenticator");
    }

    [HttpPost]
    public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            ModelState.AddModelError("model.Code", "Verification code is invalid.");
            await LoadSharedKeyAndQrCodeUriAsync(user);
            return View("EnableAuthenticator");
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        var userId = await _userManager.GetUserIdAsync(user);
        Log.Information("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

        return RedirectToAction(nameof(ShowRecoveryCodes));
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
    {
        // Load the authenticator key & QR code URI to display on the form
        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        SharedKey = FormatKey(unformattedKey);

        var email = await _userManager.GetEmailAsync(user);
        AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
    }

    private static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            _urlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"),
            _urlEncoder.Encode(email),
            unformattedKey);
    }

    public IActionResult ShowRecoveryCodes()
    {
        var recoveryCodes = (string[])TempData[RecoveryCodesKey];
        if (recoveryCodes == null)
        {
            return RedirectToAction(nameof(EnableAuthenticator));
        }

        var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
        return View("ShowRecoveryCodes", model);
    }

}
