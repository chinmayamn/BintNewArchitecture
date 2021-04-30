using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;
using Bint.Data;
using Bint.Models;
using Bint.Models.AccountViewModels;
using Bint.Services;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using DeviceDetectorNET.Parser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bint.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly DbFunc _dbf;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private readonly IMessage _message;
        private readonly IHttpContextAccessor _request;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContext,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            ApplicationDbContext context,
            IMessage message)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _request = httpContext;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _message = message;
            _dbf = new DbFunc(_logger);
        }

        [TempData] public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin model, string returnUrl = null)
        {
            try
            {
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(model.LoginViewModel.Email,
                        model.LoginViewModel.Password, model.LoginViewModel.RememberMe,true);
                    if (result.Succeeded)
                    {
                        // Resolve the user via their email
                        var user = await _userManager.FindByEmailAsync(model.LoginViewModel.Email);

                        if (!user.EmailConfirmed)
                        {
                            TempData["error"] =
                                "User account has not been confirm. Check registered email for confirmation";
                            _logger.LogError(
                                "User account has not been confirm. Check registered email for confirmation {user}",
                                user);
                            return View("login");
                        }

                        // Get the roles for the user
                        var roles = await _userManager.GetRolesAsync(user);

                        string uAgent = _request.HttpContext.Request.Headers["User-Agent"];

                        var ipv4 = "";
                        var ipv6 = "";
                        var pip = "";

                        var remoteIpAddress =
                            _request.HttpContext.Connection
                                .RemoteIpAddress; //major important one, gets public ip address
                        pip = remoteIpAddress.ToString();

                        //normally will get wifi router ip. for mobile it will throw socket exception
                        try
                        {
                            if ((await Dns.GetHostEntryAsync(remoteIpAddress)).AddressList.Any())
                            {
                                if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                                    // for mobiles normally ipv6 will not be there
                                {
                                    remoteIpAddress = (await Dns.GetHostEntryAsync(remoteIpAddress)).AddressList
                                        .First(x => x.AddressFamily ==
                                                    AddressFamily.InterNetwork);
                                    ipv6 = remoteIpAddress.ToString();
                                }


                                if (remoteIpAddress.AddressFamily == AddressFamily.InterNetwork) //
                                {
                                    remoteIpAddress = (await Dns.GetHostEntryAsync(remoteIpAddress)).AddressList.Last(x =>
                                        x.AddressFamily == AddressFamily.InterNetwork);
                                    ipv4 = remoteIpAddress.ToString();
                                }
                            }
                        }
                        catch (SocketException socket)
                        {
                            _logger.LogError(socket.ToString(), "Connected through mobile data, no dns {socket}",
                                socket);
                        }


                        DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);
                        var dd = new DeviceDetector(uAgent);
                        dd.SetCache(new DictionaryCache());
                        dd.DiscardBotInformation();
                        dd.SkipBotDetection();
                        dd.Parse();

                        var cd = new CaptureDeviceData();
                        var indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

                        cd.OsName = dd.GetOs().Match.Name;
                        cd.OsVersion = dd.GetOs().Match.Version;
                        cd.OsPlatform = dd.GetOs().Match.Platform;
                        cd.BrowserName = dd.GetBrowserClient().Match.Name;
                        cd.BrowserVersion = dd.GetBrowserClient().Match.Version;
                        cd.DeviceName = dd.GetDeviceName();
                        cd.DeviceModel = dd.GetModel();
                        cd.Brand = dd.GetBrand();
                        cd.BrandName = dd.GetBrandName();
                        cd.Useragent = uAgent;
                        cd.URole = roles[0];
                        cd.UserId = user.Id;
                        cd.LoginTime = indianTime;
                        cd.PublicIp = pip;
                        cd.Ipv4 = ipv4;
                        cd.Ipv6 = ipv6;

                        _context._captureDeviceData.Add(cd);
                        await _context.SaveChangesAsync();


                        if (roles[0].ToLower() == "admin")
                            return RedirectToAction(nameof(AdminController.Dashboard), "Admin");
                        if (roles[0].ToLower() == "investor")
                            return RedirectToAction(nameof(InvestorController.Dashboard), "Investor");
                        if (roles[0].ToLower() == "client")
                            return RedirectToAction(nameof(ClientController.Dashboard), "Client");
                        if (roles[0].ToLower() == "partner")
                            return RedirectToAction(nameof(PartnerController.Dashboard), "Partner");
                        return RedirectToLocal(returnUrl);
                    }

                    if (result.RequiresTwoFactor)
                        return RedirectToAction(nameof(LoginWith2fa), new {returnUrl, model.LoginViewModel.RememberMe});
                    if (result.IsLockedOut)
                    {
                        TempData["error"] = "User account has been locked out. Contact administrator";
                        _logger.LogError("User account has been locked out, wrong password {User}", User);
                        return View("login");
                    }

                    TempData["error"] = "Invalid username and password";
                    _logger.LogError(
                        "Invalid username and password {model.LoginViewModel.Email}{model.LoginViewModel.Password}",
                        model.LoginViewModel.Email, model.LoginViewModel.Password);
                    return View("login");
                }

                return View("login");
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred";
                _logger.LogError(ex.ToString(),
                    "Invalid model for login {model.LoginViewModel.Email}{model.LoginViewModel.Password}",
                    model.LoginViewModel.Email, model.LoginViewModel.Password);
                return View("login");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null) throw new ApplicationException("Unable to load two-factor authentication user.");

            var model = new LoginWith2faViewModel {RememberMe = rememberMe};
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe,
            string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result =
                await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe,
                    model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.{user.Id}", user.Id);
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.{user.Id}", user.Id);
                return RedirectToAction(nameof(Lockout));
            }

            _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) throw new ApplicationException("Unable to load two-factor authentication user.");

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model,
            string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) throw new ApplicationException("Unable to load two-factor authentication user.");

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded) return RedirectToLocal(returnUrl);
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.{user.Id}", user.Id);
                return RedirectToAction(nameof(Lockout));
            }

            _logger.LogWarning("Invalid recovery code entered for user with ID {user.Id}", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/Account/Error{statusCode:int}")]
        public IActionResult Error(int statusCode)
        {
            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    _logger.LogError(statusCodeData.OriginalPath);
                    break;
                case 400:
                    _logger.LogError(statusCodeData.OriginalPath);
                    break;
                case 500:
                    _logger.LogError(statusCodeData.OriginalPath);
                    break;
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var s = await _userManager.GetRolesAsync(user);
            ViewData["layout"] = s[0];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null, string quick = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

                var user = new ApplicationUser {UserName = model.Email, Email = model.Email};
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.Mobile = model.Mobile;
                user.Address = model.Address;
                user.BankName = model.BankName;
                user.BankAccount = model.BankAccount;
                user.IfscCode = model.IfscCode;
                user.AccountHolderName = model.AccountHolderName;
                user.Pan = model.Pan;
                user.UpiId = model.UpiId;
                user.Status = model.Status;
                user.CreatedBy = _userManager.GetUserAsync(User).Result.UserId;
                user.Kyc = ActivityLogEnum.Pending.ToString();

                if (model.ProfilePicture == null)
                    user.ProfilePicture = "/content/avatar.png";

                user.CreatedOn = indianTime;
                user.CreatedId = _userManager.GetUserId(User);

                //engage role id
                var f = "";
                if (quick == "true")
                {
                    f = user.UpiId;
                    user.UpiId = "";
                    user.Status = "Active";
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    var role = await _roleManager.FindByIdAsync(f); //get user role and add to his account
                    await _userManager.AddToRoleAsync(user, role.Name);
                    //  await _signInManager.SignInAsync(user, isPersistent: false);    // disabled user signing after registering

                    var z = _context.RegId.First();
                    int z1;
                    if (role.Name == "Admin")
                    {
                        z1 = Convert.ToInt32(z.AdminId);
                        z1 += 1;
                        user.UserId = "A" + z1.ToString("D4");
                        z.AdminId = z1.ToString("D4");
                    }
                    else if (role.Name == "Partner")
                    {
                        z1 = Convert.ToInt32(z.PartnerId);
                        z1 += 1;
                        user.UserId = "P" + z1.ToString("D4");
                        z.PartnerId = z1.ToString("D4");
                    }
                    else if (role.Name == "Investor")
                    {
                        z1 = Convert.ToInt32(z.InvestorId);
                        z1 += 1;
                        user.UserId = "I" + z1.ToString("D4");
                        z.InvestorId = z1.ToString("D4");
                    }
                    else if (role.Name == "Client")
                    {
                        z1 = Convert.ToInt32(z.ClientId);
                        z1 += 1;
                        user.UserId = "C" + z1.ToString("D4");
                        z.ClientId = z1.ToString("D4");
                    }

                    await _context.SaveChangesAsync();
                    //update reg id
                    await _userManager.UpdateAsync(user);

                    var activityLog = new ActivityLog
                    {
                        Userid = user.CreatedBy,
                        ActivityType = ActivityLogEnum.Person.ToString(),
                        ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                        Activity = "Created user " + user.UserId
                    };
                    _context.ActivityLog.Add(activityLog);
                    await _context.SaveChangesAsync();

                    TempData["data"] = "User has been created successfully";
                    return RedirectToLocal(returnUrl);
                }

                AddErrors(result);
                TempData["data"] = "Error occurred while creating user";
                _logger.LogError("Error occurred while creating user");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [ActionName("Logout")]
        public IActionResult Logouts()
        {
            return View("logout");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var remoteIpAddress =
                _request.HttpContext.Connection.RemoteIpAddress; //major important one, gets public ip address
            var pip = remoteIpAddress.ToString();
            var id = _userManager.GetUserId(User);
            var stu = _context._captureDeviceData.Where(j => j.UserId == id && j.PublicIp == pip).LastOrDefault();
            var indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
            stu.LogoutTime = indianTime;
            await _context.SaveChangesAsync();
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login), "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new {returnUrl});
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
            if (info == null) return RedirectToAction(nameof(Login));

            // Sign in the user with this external login provider if the user already has a login.
            var result =
                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
            if (result.Succeeded) return RedirectToLocal(returnUrl);
            if (result.IsLockedOut) return RedirectToAction(nameof(Lockout));

            // If the user does not have an account, then ask the user to create an account.
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = info.LoginProvider;
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            return View("ExternalLogin", new ExternalLoginViewModel {Email = email});
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model,
            string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                    throw new ApplicationException("Error loading external login information during confirmation.");
                var user = new ApplicationUser {UserName = model.Email, Email = model.Email};
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);

                        return RedirectToLocal(returnUrl);
                    }
                }

                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null) return RedirectToAction(nameof(Login), "Home");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpGet]
        public IActionResult SaveProfile(string id, string firstname, string lastname, string mobile, string address,
            string bankName, string ifsc, string accountHolder, string pan, string upi, string bankAccount)
        {
            try
            {
                var appuser = _userManager.FindByIdAsync(id).Result;
                appuser.Firstname = firstname;
                appuser.Lastname = lastname;
                appuser.Mobile = mobile;
                appuser.Address = address;
                appuser.BankName = bankName;
                appuser.IfscCode = ifsc;
                appuser.AccountHolderName = accountHolder;
                appuser.Pan = pan;
                appuser.UpiId = upi;
                appuser.BankAccount = bankAccount;

                var result = _userManager.UpdateAsync(appuser).Result;
                if (result.Succeeded)
                    return Json("success");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString(), "User update failed");
                return Json("User update failed");
            }

            return Json("");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                TempData["error"] = "Cant find account for this user";
                _logger.LogError("Cant find account for this user {model.Email}", model.Email);
                return RedirectToAction(nameof(Login));
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                TempData["error"] = "Account not verified, check your email for confirm email";
                _logger.LogError("Account not verified, check your email for confirm email {model.Email}",
                    model.Email);
                return RedirectToAction(nameof(Login));
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
            await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
            TempData["data"] = "Please check your email to reset your password";
            return RedirectToAction(nameof(Login));

            // If we got this far, something failed, redisplay form
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
            if (code == null) throw new ApplicationException("A code must be supplied for password reset.");
            var model = new ResetPasswordViewModel {Code = code};
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded) return RedirectToAction(nameof(ResetPasswordConfirmation));
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AccessDenied()
        {
            string uagent = _request.HttpContext.Request.Headers["User-Agent"];
            var rUrl = "";
            if (HttpContext.Request.Query["returnurl"].ToString() != "")
                rUrl = HttpContext.Request.Query["returnurl"].ToString();

            var id = "";
            var cRole = "";
            if (_userManager.GetUserId(User) != "")
            {
                id = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(id);
                var roles = await _userManager.GetRolesAsync(user);
                cRole = roles[0];
            }

            var ipv4 = "";
            var ipv6 = "";
            var pip = "";

            var remoteIpAddress =
                _request.HttpContext.Connection.RemoteIpAddress; //major important one, gets public ip address
            pip = remoteIpAddress.ToString();

            //normally will get wifi router ip
            if ((await Dns.GetHostEntryAsync(remoteIpAddress)).AddressList.Any())
            {
                if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6) //
                {
                    remoteIpAddress = (await Dns.GetHostEntryAsync(remoteIpAddress)).AddressList
                        .First(x => x.AddressFamily == AddressFamily.InterNetwork);
                    ipv6 = remoteIpAddress.ToString();
                }


                if (remoteIpAddress.AddressFamily == AddressFamily.InterNetwork) //
                {
                    remoteIpAddress = (await Dns.GetHostEntryAsync(remoteIpAddress)).AddressList
                        .Last(x => x.AddressFamily == AddressFamily.InterNetwork);
                    ipv4 = remoteIpAddress.ToString();
                }
            }


            DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);
            var dd = new DeviceDetector(uagent);
            dd.SetCache(new DictionaryCache());
            dd.DiscardBotInformation();
            dd.SkipBotDetection();
            dd.Parse();

            var cd = new RestrictedAccess();
            var indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
            cd.OsName = dd.GetOs().Match.Name;
            cd.OsVersion = dd.GetOs().Match.Version;
            cd.OsPlatform = dd.GetOs().Match.Platform;
            cd.BrowserName = dd.GetBrowserClient().Match.Name;
            cd.BrowserVersion = dd.GetBrowserClient().Match.Version;
            cd.DeviceName = dd.GetDeviceName();
            cd.DeviceModel = dd.GetModel();
            cd.Brand = dd.GetBrand();
            cd.BrandName = dd.GetBrandName();
            cd.Useragent = uagent;
            cd.Urole = cRole;
            cd.UserId = id;
            cd.ErrorTime = indianTime;
            cd.PublicIp = pip;
            cd.Ipv4 = ipv4;
            cd.Ipv6 = ipv6;
            cd.Verified = "Not Verified";
            cd.ReturnUrl = rUrl;
            _context._restrictedAccess.Add(cd);
            await _context.SaveChangesAsync();
            await _signInManager.SignOutAsync(); //remove session variables
            _logger.LogError("Access denied {rUrl}", rUrl);
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> SetManualLock(string uid, bool status)
        {
            try
            {
                var u = await _userManager.FindByIdAsync(uid);

                await _userManager.SetLockoutEnabledAsync(u, true);
                if (status)
                    await _userManager.SetLockoutEndDateAsync(u, DateTimeOffset.MaxValue);
                else
                    await _userManager.SetLockoutEndDateAsync(u, null);
                return Json(new {data = "success"});
            }
            catch (Exception)
            {
                return Json("");
            }
        }

        [HttpGet]
        public async Task<ActionResult> SetManualVerification(string uid, bool status)
        {
            try
            {
                var u = await _userManager.FindByIdAsync(uid);

                //   await _userManager.SetLockoutEnabledAsync(u, true);
                if (status)
                    u.EmailConfirmed = true;
                else u.EmailConfirmed = false;

                await _userManager.UpdateAsync(u);
                return Json(new {data = "success"});
            }
            catch (Exception)
            {
                return Json("");
            }
        }

        [HttpGet]
        [Route("/account/DeleteDocs")]
        public IActionResult DeleteDocs(int id)
        {
            try
            {
                var s = _context.Doc.First(x => x.Id == id);
                var z = Directory.GetCurrentDirectory();
                var t = "";
                t = z + "\\wwwroot" + s.DocPath.Replace("/", "\\");
                var fileInfo = new FileInfo(t);
                if (fileInfo.Exists)
                    fileInfo.Delete();
                _context.Doc.Remove(s);
                _context.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "Document hard delete error");
                TempData["error"] = "Document delete error";
                return Json("");
            }
        }

        [HttpGet]
        [Route("/account/userprofile/{id}")]
        public async Task<IActionResult> UserProfile(string id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var s = await _userManager.GetRolesAsync(user);
                ViewData["layout"] = s[0];

                var aupd = new AdminUserProfileDashboard();
                var u = _userManager.FindByIdAsync(id).Result;
                aupd.UserProfile = u;
                var adb = new ActivityLogDashboard
                {
                    ActivityLogTable = _dbf.GetUserActivityLog(u.UserId)
                };
                aupd.ActivityLogDashboard = adb;
                aupd.UserDocs = _dbf.GetKycDocs(id);
                aupd.UserList = _userManager.Users.Where(x => x.CreatedBy == u.UserId);
                return View(aupd);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(nameof(ClientController.Plans), "Client");
        }

        #endregion
    }
}