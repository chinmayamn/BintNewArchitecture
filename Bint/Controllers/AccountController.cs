﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bint.Models;
using Bint.Models.AccountViewModels;
using Bint.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Sockets;
using DeviceDetectorNET;
using DeviceDetectorNET.Parser;
using DeviceDetectorNET.Cache;
using Bint.Data;
using System.IO;
using Microsoft.AspNetCore.Diagnostics;

namespace Bint.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private IHttpContextAccessor _request;
        private IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly IMessage _message;
        DBFunc dbf;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpcontext,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            ApplicationDbContext context,
            IMessage message)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _request = httpcontext;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _message = message;
            dbf = new DBFunc(_logger);
        }

        [TempData]
        public string ErrorMessage { get; set; }

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
                    var result = await _signInManager.PasswordSignInAsync(model.loginviewmodel.Email, model.loginviewmodel.Password, model.loginviewmodel.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        // Resolve the user via their email
                        var user = await _userManager.FindByEmailAsync(model.loginviewmodel.Email);

                        if (!(user.EmailConfirmed))
                        {
                            TempData["error"] = "User account has not been confirm. Check registered email for confirmation";
                            _logger.LogError("User account has not been confirm. Check registered email for confirmation {user}",user);
                            return View("login");
                        }
                        else
                        { 
                            // Get the roles for the user
                            var roles = await _userManager.GetRolesAsync(user);

                            string uagent = _request.HttpContext.Request.Headers["User-Agent"];

                            string ipv4 = "";
                            string ipv6 = "";
                            string pip = "";

                            IPAddress remoteIPAddress = _request.HttpContext.Connection.RemoteIpAddress; //major important one, gets public ip address
                            pip = remoteIPAddress.ToString();

                            //normally will get wifi router ip. for mobile it will throw socket exception
                            try
                            {
                                if (System.Net.Dns.GetHostEntry(remoteIPAddress).AddressList.Count() > 0)
                                {
                                    if (remoteIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                                    // for mobiles normally ipv6 will not be there
                                    {

                                        remoteIPAddress = System.Net.Dns.GetHostEntry(remoteIPAddress).AddressList
                                            .First(x => x.AddressFamily ==
                                                        System.Net.Sockets.AddressFamily.InterNetwork);
                                        ipv6 = remoteIPAddress.ToString();

                                    }


                                    if (remoteIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) //
                                    {

                                        remoteIPAddress = System.Net.Dns.GetHostEntry(remoteIPAddress).AddressList.Last(x =>
                                            x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                                        ipv4 = remoteIPAddress.ToString();
                                    }
                                }
                            }
                            catch (SocketException socket)
                            {
                                _logger.LogError(socket.ToString(), "Connected through mobile data, no dns {socket}", socket);
                            }



                            DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);
                            var dd = new DeviceDetector(uagent);
                            dd.SetCache(new DictionaryCache());
                            dd.DiscardBotInformation();
                            dd.SkipBotDetection();
                            dd.Parse();

                            CaptureDeviceData cd = new CaptureDeviceData();
                            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                            cd.OSName = dd.GetOs().Match.Name.ToString();
                            cd.OSVersion = dd.GetOs().Match.Version.ToString();
                            cd.OSPlatform = dd.GetOs().Match.Platform.ToString();
                            cd.BrowserName = dd.GetBrowserClient().Match.Name;
                            cd.BrowserVersion = dd.GetBrowserClient().Match.Version;
                            cd.DeviceName = dd.GetDeviceName().ToString();
                            cd.DeviceModel = dd.GetModel().ToString();
                            cd.Brand = dd.GetBrand().ToString();
                            cd.BrandName = dd.GetBrandName().ToString();
                            cd.Useragent = uagent;
                            cd.urole = roles[0];
                            cd.userid = user.Id;
                            cd.LoginTime = indianTime;
                            cd.PublicIp = pip;
                            cd.IPv4 = ipv4;
                            cd.IPv6 = ipv6;

                            _context._captureDeviceData.Add(cd);
                            _context.SaveChanges();


                            if (roles[0].ToLower() == "admin")
                                return RedirectToAction(nameof(AdminController.Dashboard), "Admin");
                            else if (roles[0].ToLower() == "investor")
                                return RedirectToAction(nameof(InvestorController.Dashboard), "Investor");
                            else if (roles[0].ToLower() == "client")
                                return RedirectToAction(nameof(ClientController.Dashboard), "Client");
                            else if (roles[0].ToLower() == "partner")
                                return RedirectToAction(nameof(PartnerController.Dashboard), "Partner");
                            else
                                return RedirectToLocal(returnUrl);
                        }
                        
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.loginviewmodel.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        TempData["error"] = "User account has been locked out. Contact administrator";
                        _logger.LogError("User account has been locked out, wrong password {User}", User);
                        return View("login");
                    }
                    else
                    {
                        TempData["error"] = "Invalid username and password";
                        _logger.LogError("Invalid username and password {model.loginviewmodel.Email}{model.loginviewmodel.Password}", model.loginviewmodel.Email, model.loginviewmodel.Password);
                        return View("login");
                    }

                }
                return View("login");
            }
            catch(Exception ex)
            {
                TempData["error"] = "An error occurred";
                _logger.LogError(ex.ToString(), "Invalid model for login {model.loginviewmodel.Email}{model.loginviewmodel.Password}", model.loginviewmodel.Email, model.loginviewmodel.Password);
                return View("login");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

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

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.{user.Id}", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.{user.Id}", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

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

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
             
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.{user.Id}", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {user.Id}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/Account/Error{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch(statusCode)
            {
                case 404:_logger.LogError(statusCodeData.OriginalPath);break;
                case 400:_logger.LogError(statusCodeData.OriginalPath);break;
                case 500:_logger.LogError(statusCodeData.OriginalPath);break;
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
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null,string quick=null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.Mobile = model.Mobile;
                user.Address = model.Address;
                user.Bankname = model.Bankname;
                user.Bankaccount = model.Bankaccount;
                user.Ifsc_code = model.Ifsc_code;
                user.Account_holder_name = model.Account_holder_name;
                user.Pan = model.Pan;
                user.Upi_id = model.Upi_id;
                user.Status = model.Status;
                user.Created_by = _userManager.GetUserAsync(User).Result.UserId;
                user.KYC = ActivityLogEnum.Pending.ToString();

                if (model.Profilepicture==null)
                    user.Profilepicture = "/content/avatar.png";
                
                user.Created_on = indianTime;
                user.Created_id = _userManager.GetUserId(User);

                //engage role id
                string f = "";
                if (quick == "true")
                {
                    f = user.Upi_id;
                    user.Upi_id = "";
                    user.Status = "Active";
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);  

                    IdentityRole role = await _roleManager.FindByIdAsync(f);  //get user role and add to his account
                    await _userManager.AddToRoleAsync(user,role.Name);
                    //  await _signInManager.SignInAsync(user, isPersistent: false);    // disabled user signing after registering

                    var z = _context.regId.First();
                    var z1=0;
                    if (role.Name == "Admin")
                    {
                        z1 = Convert.ToInt32(z.AdminId); z1 = z1 + 1; user.UserId = "A" + z1.ToString("D4"); z.AdminId =z1.ToString("D4");
                    }
                    else if (role.Name == "Partner")
                    {
                        z1 = Convert.ToInt32(z.PartnerId); z1 = z1 + 1; user.UserId = "P" + z1.ToString("D4"); z.PartnerId =z1.ToString("D4");
                    }
                    else if (role.Name == "Investor")
                    {
                        z1 = Convert.ToInt32(z.InvestorId); z1 = z1 + 1; user.UserId = "I" + z1.ToString("D4"); z.InvestorId =z1.ToString("D4");
                    }
                    else if (role.Name == "Client")
                    {
                        z1 = Convert.ToInt32(z.ClientId); z1 = z1 + 1; user.UserId = "C" + z1.ToString("D4"); z.ClientId =z1.ToString("D4");
                    }
                    else
                    { }

                    
                    _context.SaveChanges();
                    //update reg id
                    await _userManager.UpdateAsync(user);

                    ActivityLog activityLog = new ActivityLog();
                    activityLog.Userid = user.Created_by;
                    activityLog.ActivityType = ActivityLogEnum.Person.ToString();
                    activityLog.ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    activityLog.Activity = "Created user " + user.UserId;
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    TempData["data"] = "User has been created successfully";
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result); TempData["data"] = "Error occurred while creating user";_logger.LogError("Error occurred while creating user");
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
            
            IPAddress remoteIPAddress = _request.HttpContext.Connection.RemoteIpAddress; //major important one, gets public ip address
            string pip = remoteIPAddress.ToString();
            var id = _userManager.GetUserId(User);
            var stu = _context._captureDeviceData.Where(j => (j.userid == id) && (j.PublicIp == pip)).LastOrDefault();
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            stu.LogoutTime = indianTime;
            _context.SaveChanges();
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
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

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                       
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
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
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
        public IActionResult SaveProfile(string id,string firstname,string lastname,string mobile, string address, string bankname, string ifsc, string accountholder, string pan, string upi,string bankaccount )
        {
            try
            {
                ApplicationUser appuser = _userManager.FindByIdAsync(id).Result;
                appuser.Firstname = firstname;
                appuser.Lastname = lastname;
                appuser.Mobile = mobile;
                appuser.Address = address;
                appuser.Bankname = bankname;
                appuser.Ifsc_code = ifsc;
                appuser.Account_holder_name = accountholder;
                appuser.Pan = pan;
                appuser.Upi_id = upi;
                appuser.Bankaccount = bankaccount;

                var result =  _userManager.UpdateAsync(appuser).Result;
                if (result.Succeeded)
                              return Json("success");
               
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString(),"User update failed");
                return Json("User update failed");
            }
            return Json("");

        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    TempData["error"] = "Cant find account for this user";
                    _logger.LogError("Cant find account for this user {model.Email}", model.Email);
                    return RedirectToAction(nameof(Login));
                }
                if(!(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                    TempData["error"] = "Account not verified, check your email for confirm email";
                    _logger.LogError("Account not verified, check your email for confirm email {model.Email}", model.Email);
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
            }

            // If we got this far, something failed, redisplay form
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
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
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

        [HttpGet]
        public async Task<IActionResult> AccessDenied()
        {
            string uagent = _request.HttpContext.Request.Headers["User-Agent"];
            string rurl = "";
            if(HttpContext.Request.Query["returnurl"].ToString() !="")
            {
                rurl = HttpContext.Request.Query["returnurl"].ToString();
            }

            var id = "";
            var crole = "";
            if(_userManager.GetUserId(User) !="")
            {
               
                id = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(id);
                var roles = await _userManager.GetRolesAsync(user);
                crole = roles[0];
            }

            string ipv4 = "";
            string ipv6 = "";
            string pip = "";

            IPAddress remoteIPAddress = _request.HttpContext.Connection.RemoteIpAddress; //major important one, gets public ip address
            pip = remoteIPAddress.ToString();

            //normally will get wifi router ip
            if (System.Net.Dns.GetHostEntry(remoteIPAddress).AddressList.Count() > 0)
            {
                if (remoteIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) //
                {

                    remoteIPAddress = System.Net.Dns.GetHostEntry(remoteIPAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    ipv6 = remoteIPAddress.ToString();
                }



                if (remoteIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) //
                {

                    remoteIPAddress = System.Net.Dns.GetHostEntry(remoteIPAddress).AddressList.Last(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    ipv4 = remoteIPAddress.ToString();
                }
            }



            DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);
            var dd = new DeviceDetector(uagent);
            dd.SetCache(new DictionaryCache());
            dd.DiscardBotInformation();
            dd.SkipBotDetection();
            dd.Parse();

            RestrictedAccess cd = new RestrictedAccess();
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            cd.OSName = dd.GetOs().Match.Name.ToString();
            cd.OSVersion = dd.GetOs().Match.Version.ToString();
            cd.OSPlatform = dd.GetOs().Match.Platform.ToString();
            cd.BrowserName = dd.GetBrowserClient().Match.Name;
            cd.BrowserVersion = dd.GetBrowserClient().Match.Version;
            cd.DeviceName = dd.GetDeviceName().ToString();
            cd.DeviceModel = dd.GetModel().ToString();
            cd.Brand = dd.GetBrand().ToString();
            cd.BrandName = dd.GetBrandName().ToString();
            cd.Useragent = uagent;
            cd.urole = crole;
            cd.userid = id;
            cd.ErrorTime = indianTime;
            cd.PublicIp = pip;
            cd.IPv4 = ipv4;
            cd.IPv6 = ipv6;
            cd.Verified = "Not Verified";
            cd.ReturnUrl = rurl;
            _context._restrictedAccess.Add(cd);
            _context.SaveChanges();
            await _signInManager.SignOutAsync();//remove session variables
            _logger.LogError("Access denied {rurl}", rurl);
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> SetManualLock(string uid, bool status)
        {
            try
            {
                ApplicationUser u = await _userManager.FindByIdAsync(uid);
                 
                await _userManager.SetLockoutEnabledAsync(u, true);
                if(status)
                   await _userManager.SetLockoutEndDateAsync(u, DateTimeOffset.MaxValue);
                else
                    await _userManager.SetLockoutEndDateAsync(u, null);
                return Json(new { data = "success" });
            }
            catch(Exception ex)
            {
                return Json("");
            }
            
            
        }

        [HttpGet]
        public async Task<ActionResult> SetManualVerification(string uid, bool status)
        {
            try
            {
                ApplicationUser u = await _userManager.FindByIdAsync(uid);

             //   await _userManager.SetLockoutEnabledAsync(u, true);
                if (status)
                    u.EmailConfirmed = true;
                else    u.EmailConfirmed = false;

                await _userManager.UpdateAsync(u);
                return Json(new { data = "success" });
            }
            catch (Exception ex)
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
                Doc s = _context.Doc.Where(x => x.Id == id).First();
                var z = Directory.GetCurrentDirectory(); var t = ""; FileInfo fileInfo;
                t = z + "\\wwwroot" + s.Docpath.Replace("/", "\\");
                fileInfo = new System.IO.FileInfo(t);
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

                AdminUserProfileDashboard aupd = new AdminUserProfileDashboard();
                ApplicationUser u = _userManager.FindByIdAsync(id).Result;
                aupd._userProfile = u;
                ActivityLogDashboard adb = new ActivityLogDashboard();
                adb.activityLogTable = dbf.GetUserActivityLog(u.UserId);
                aupd._activityLogDashboard = adb;
                aupd.UserDocs = dbf.GetKYCDocs(id);
                aupd.UserList = _userManager.Users.Where(x => x.Created_by == u.UserId);
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
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {   
              
                return RedirectToAction(nameof(ClientController.Index), "Client");
            }
        }

        #endregion
    }
}
