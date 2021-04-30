using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Bint.Data;
using Bint.Models;
using Bint.Models.ManageViewModels;
using Bint.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bint.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IMessage _message;
        private readonly ILogger<Message> _messageLogger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ILogger<ManageController> logger,
            UrlEncoder urlEncoder, ApplicationDbContext context, IMessage message, ILogger<Message> messageLogger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _roleManager = roleManager;
            _context = context;
            _message = message;
            _messageLogger = messageLogger;
        }

        [TempData] public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                    throw new ApplicationException(
                        $"Unexpected error occurred setting email for user with ID '{user.Id}'.");
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                    throw new ApplicationException(
                        $"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("/admin/changepassword")]
        [Route("/investor/changepassword")]
        [Route("/client/changepassword")]
        [Route("/partner/changepassword")]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            var s = await _userManager.GetRolesAsync(user);
            ViewData["layout"] = s[0];
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword) return RedirectToAction(nameof(SetPassword));

            var model = new ChangePasswordViewModel {StatusMessage = StatusMessage};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/changepassword")]
        [Route("/investor/changepassword")]
        [Route("/client/changepassword")]
        [Route("/partner/changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                if (!ModelState.IsValid) return View(model);

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

                var changePasswordResult =
                    await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    AddErrors(changePasswordResult);
                    TempData["error"] = changePasswordResult.Errors.ToString();
                    return RedirectToAction("myprofile", route);
                }

                await _signInManager.SignInAsync(user, false);
                //_logger.LogInformation("User changed their password successfully.");
                StatusMessage = "Your password has been changed.";
                //TempData["data"] = "Your password has been changed";
                return RedirectToAction("changepassword", route);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString(), "Password change error", model);
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword) return RedirectToAction(nameof(ChangePassword));

            var model = new SetPasswordViewModel {StatusMessage = StatusMessage};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var model = new ExternalLoginsViewModel {CurrentLogins = await _userManager.GetLoginsAsync(user)};
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
            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl,
                    _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
                throw new ApplicationException(
                    $"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
                throw new ApplicationException(
                    $"Unexpected error occurred adding external login for user with ID '{user.Id}'.");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
                throw new ApplicationException(
                    $"Unexpected error occurred removing external login for user with ID '{user.Id}'.");

            await _signInManager.SignInAsync(user, false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!user.TwoFactorEnabled)
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");

            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var model = new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Verification code is invalid.");
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (string[]) TempData[RecoveryCodesKey];
            if (recoveryCodes == null) return RedirectToAction(nameof(TwoFactorAuthentication));

            var model = new ShowRecoveryCodesViewModel {RecoveryCodes = recoveryCodes};
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!user.TwoFactorEnabled)
                throw new ApplicationException(
                    $"Cannot generate recovery codes for user with ID '{user.Id}' because they do not have 2FA enabled.");

            return View(nameof(GenerateRecoveryCodes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!user.TwoFactorEnabled)
                throw new ApplicationException(
                    $"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            var model = new ShowRecoveryCodesViewModel {RecoveryCodes = recoveryCodes.ToArray()};

            return View(nameof(ShowRecoveryCodes), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/debitcreditusd")]
        [Route("/investor/debitcreditusd")]
        [Route("/client/debitcreditusd")]
        [Route("/partner/debitcreditusd")]
        public async Task<IActionResult> Usd(string amount)
        {
            var mm = new Message(_messageLogger);
            var route = Request.Path.Value.Split("/")[1]; //get current user
            try
            {
                var ud = await _userManager.GetUserAsync(User);

                if (ud.Usd >= Convert.ToDecimal(amount)) //transfer if having amount
                {
                    /**************** crediting action **************/

                    var dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
                    var tusd = new TransferUsd
                    {
                        Amount = Convert.ToDecimal(amount),
                        FromUserId = ud.UserId, //creator lower level
                        ToUserId = ud.CreatedBy, //request higher level
                        RequestedDate = dt,
                        TransferDate = dt,
                        FromStatus = TransferUsdStatusEnum.Debit.ToString(),
                        ToStatus = TransferUsdStatusEnum.Credit.ToString(),
                        Userid = ud.Id
                    }; //crediting action to user

                    var sendu = await _userManager.FindByIdAsync(ud.CreatedId); //get his account
                    //update his wallet
                    sendu.Usd = sendu.Usd + tusd.Amount;
                    await _userManager.UpdateAsync(sendu);

                    ud.Usd -= tusd.Amount;
                    await _userManager.UpdateAsync(ud);


                    tusd.FromTotalAmount = ud.Usd;
                    tusd.ToTotalAmount = sendu.Usd;
                    _context.TransferUsd.Add(tusd);
                    _context.SaveChanges();

                    var activityLog = new ActivityLog
                    {
                        Userid = tusd.ToUserId,
                        ActivityDate = dt,
                        ActivityType = ActivityLogEnum.Credit.ToString(),
                        Activity = "Credited " + amount + " Usd, received from " + tusd.FromUserId +
                                           ". Balance : " + sendu.Usd
                    };
                    _context.ActivityLog.Add(activityLog);
                    _context.SaveChanges();

                    //sending message to himself
                    mm.EmailMessageBody = activityLog.Activity;
                    mm.SmsMessageBody = activityLog.Activity;
                    mm.MobileNumber = sendu.Mobile;
                    mm.To = sendu.Email;
                    mm.Subject = "Usd Credited";
                    _message.SendMessage(mm);
                    //activity ends here

                    /**************** crediting action ends here **************/

                    /**************** debiting action **************/

                    ////update debit in his wallet
                    //tusd = new TransferUSD();
                    //tusd.Amount = Convert.ToDecimal(amount);
                    //tusd.FromUserId = ud.UserId;
                    //tusd.ToUserId = ud.CreatedBy;
                    //tusd.TransferDate = dt;
                    //tusd.RequestedDate = dt;
                    //tusd.Status = TransferUSDStatusEnum.Debit.ToString();
                    //tusd.Userid = ud.Id;


                    //tusd.TotalAmount = ud.Usd;
                    //_context.transferusd.Add(tusd);
                    //_context.SaveChanges();
                    //update ends here

                    activityLog = new ActivityLog
                    {
                        Userid = tusd.FromUserId,
                        ActivityType = ActivityLogEnum.Debit.ToString(),
                        ActivityDate = dt,
                        Activity = "Debited " + amount + " Usd, transferred to " + tusd.ToUserId +
                                   ". Balance : " + ud.Usd
                    };
                    _context.ActivityLog.Add(activityLog);
                    _context.SaveChanges();

                    //sending message to himself
                    mm.EmailMessageBody = activityLog.Activity;
                    mm.SmsMessageBody = activityLog.Activity;
                    mm.MobileNumber = ud.Mobile;
                    mm.To = ud.Email;
                    mm.Subject = "Usd Debited";
                    _message.SendMessage(mm);

                    /**************** debiting action ends here **************/
                    TempData["data"] = activityLog.Activity;
                    return RedirectToAction("Usd", route);
                }

                TempData["error"] = "Insufficient amount in Usd wallet to transfer";
                return RedirectToAction("Usd", route);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData["error"] = "Error occurred while payback Usd";
                return View("Usd", route);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/requestusd")]
        [Route("/investor/requestusd")]
        [Route("/client/requestusd")]
        [Route("/partner/requestusd")]
        public async Task<IActionResult> RequestUsd(string amount)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                var ud = await _userManager.GetUserAsync(User);
                var tusd = new TransferUsd
                {
                    Amount = Convert.ToDecimal(amount),
                    FromUserId = ud.UserId,
                    ToUserId = ud.CreatedBy,
                    RequestedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                    TransferDate = null
                };
                tusd.FromStatus = tusd.ToStatus = TransferUsdStatusEnum.Requested.ToString();
                tusd.Userid = ud.Id;
                _context.TransferUsd.Add(tusd);
                _context.SaveChanges();

                var activityLog = new ActivityLog
                {
                    Userid = tusd.FromUserId,
                    ActivityType = ActivityLogEnum.RequestUsd.ToString(),
                    ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                    Activity = "Requested " + amount + " Usd to " + ud.CreatedBy
                };
                _context.ActivityLog.Add(activityLog);
                _context.SaveChanges();

                var mm = new Message(_messageLogger)
                {
                    EmailMessageBody = activityLog.Activity,
                    SmsMessageBody = activityLog.Activity,
                    MobileNumber = ud.Mobile,
                    To = ud.Email,
                    Subject = "Usd Request"
                }; //sending message to himself
                _message.SendMessage(mm);


                var sendu = await _userManager.FindByIdAsync(tusd.Userid); //sending message to requested person
                mm = new Message(_messageLogger)
                {
                    EmailMessageBody = ud.UserId + " has requested " + amount + " Usd",
                    SmsMessageBody = ud.UserId + " has requested " + amount + " Usd",
                    MobileNumber = sendu.Mobile,
                    To = sendu.Email,
                    Subject = "Usd Requested"
                };
                _message.SendMessage(mm);

                activityLog = new ActivityLog
                {
                    Userid = tusd.ToUserId,
                    ActivityType = ActivityLogEnum.RequestUsd.ToString(),
                    ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                    Activity = ud.UserId + " has requested " + amount + " Usd"
                };
                _context.ActivityLog.Add(activityLog);
                _context.SaveChanges();


                TempData["data"] = "Requested " + amount + " Usd to " + ud.CreatedBy;
                return RedirectToAction("Usd", route);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData["error"] = "Error occurred while requesting Usd";
            }

            return View("Usd");
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        [Route("/admin/transferusd")]
        [Route("/investor/transferusd")]
        [Route("/client/transferusd")]
        [Route("/partner/transferusd")]
        public async Task<IActionResult> TransferUsd(int id, string act)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                var ud = await _userManager.GetUserAsync(User); //get own wallet details

                var tusd = _context.TransferUsd.First(x => x.Id == id);

                if (act == "Transfer")
                {
                    if (ud.Usd >= tusd.Amount) //transfer if having amount
                    {
                        var mm = new Message(_messageLogger);
                        var activityLog = new ActivityLog();

                        /**************** crediting action starts here **************/

                        tusd.TransferDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
                        tusd.FromStatus = TransferUsdStatusEnum.Received.ToString();
                        tusd.ToStatus = TransferUsdStatusEnum.Transferred.ToString();
                        // _context.transferusd.Add(tusd);

                        var uid = await _userManager.FindByIdAsync(tusd.Userid); //update in user -  credit
                        uid.Usd += tusd.Amount;
                        await _userManager.UpdateAsync(uid);

                        ud.Usd -= tusd.Amount; //update in himself - debit
                        await _userManager.UpdateAsync(ud);

                        tusd.FromTotalAmount = uid.Usd;
                        tusd.ToTotalAmount = ud.Usd;
                        _context.SaveChanges();

                        activityLog = new ActivityLog
                        {
                            Userid = tusd.FromUserId,
                            ActivityType = ActivityLogEnum.ReceiveUsd.ToString(),
                            ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                            Activity = "Received " + tusd.Amount + " Usd from " + tusd.ToUserId +
                                               ". Balance : " + uid.Usd
                        }; //set activitylog
                        _context.ActivityLog.Add(activityLog);
                        _context.SaveChanges();

                        mm = new Message(_messageLogger)
                        {
                            EmailMessageBody = ud.UserId + " has transferred " + tusd.Amount + " Usd",
                            SmsMessageBody = ud.UserId + " has transferred " + tusd.Amount + " Usd",
                            MobileNumber = uid.Mobile,
                            To = uid.Email,
                            Subject = "Usd Received"
                        }; //sending message to requested person
                        _message.SendMessage(mm);

                        /**************** crediting action ends here **************/

                        /**************** debiting action starts here **************/


                        //TransferUSD stusd = new TransferUSD();  //dont add, will create duplicate and all problems will come
                        //stusd.Amount = tusd.Amount;
                        //stusd.TotalAmount = ud.Usd;
                        //stusd.FromUserId = ud.UserId;
                        //stusd.ToUserId = tusd.ToUserId;
                        //stusd.TransferDate = tusd.TransferDate;
                        //stusd.Status = TransferUSDStatusEnum.Transferred.ToString();
                        //stusd.Userid = ud.Id;
                        //stusd.RequestedDate = tusd.RequestedDate;


                        //_context.transferusd.Add(stusd);
                        //_context.SaveChanges();


                        activityLog = new ActivityLog
                        {
                            Userid = tusd.ToUserId,
                            ActivityType = ActivityLogEnum.TransferUsd.ToString(),
                            ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                            Activity = "Transferred " + tusd.Amount + " Usd to " + tusd.FromUserId +
                                               ". Balance : " + ud.Usd
                        }; //set activitylog
                        _context.ActivityLog.Add(activityLog);
                        _context.SaveChanges();

                        mm = new Message(_messageLogger)
                        {
                            EmailMessageBody = activityLog.Activity,
                            SmsMessageBody = activityLog.Activity,
                            MobileNumber = ud.Mobile,
                            To = ud.Email,
                            Subject = "Usd Transferred"
                        }; //sending message to himself
                        _message.SendMessage(mm);

                        /**************** debiting action ends here **************/

                        TempData["data"] = activityLog.Activity;
                        return Json("success");
                    }

                    // TempData["error"] = "Insufficient amount in Usd wallet";
                    return Json("Insufficient amount in Usd wallet");
                }

                if (act == "Reject")
                {
                    tusd.FromStatus = tusd.ToStatus = TransferUsdStatusEnum.Rejected.ToString();
                    tusd.TransferDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
                    _context.SaveChanges();

                    var activityLog = new ActivityLog
                    {
                        Userid = tusd.ToUserId,
                        ActivityType = ActivityLogEnum.Reject.ToString(),
                        ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                        Activity = "Rejected " + tusd.Amount + " Usd to " + tusd.FromUserId
                    }; //set activitylog
                    _context.ActivityLog.Add(activityLog);
                    _context.SaveChanges();

                    var mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = activityLog.Activity,
                        SmsMessageBody = activityLog.Activity,
                        MobileNumber = ud.Mobile,
                        To = ud.Email,
                        Subject = "Usd Rejected"
                    }; //sending message to himself
                    _message.SendMessage(mm);

                    var uid = await _userManager.FindByIdAsync(tusd.Userid); //sending message to requested person

                    mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = ud.UserId + " has rejected " + tusd.Amount + " Usd to transfer",
                        SmsMessageBody = ud.UserId + " has transferred " + tusd.Amount + " Usd to transfer",
                        MobileNumber = uid.Mobile,
                        To = uid.Email,
                        Subject = "Usd Rejected"
                    };
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog
                    {
                        Userid = uid.UserId,
                        ActivityType = ActivityLogEnum.Reject.ToString(),
                        ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                        Activity = ud.UserId + " has rejected " + tusd.Amount + " Usd to transfer"
                    }; //set activitylog
                    _context.ActivityLog.Add(activityLog);
                    _context.SaveChanges();

                    TempData["data"] = activityLog.Activity;
                    return Json("success");
                }

                return RedirectToAction("Usd", route);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData["error"] = "Error occurred while requesting Usd";
            }

            return View("Usd");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/withdrawusd")]
        [Route("/investor/withdrawusd")]
        [Route("/client/withdrawusd")]
        [Route("/partner/withdrawusd")]
        public async Task<IActionResult> WithdrawUsd(string withdrawAmount)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                var ud = await _userManager.GetUserAsync(User);
                if (ud.Usd >= Convert.ToDecimal(withdrawAmount))
                {
                    var dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

                    var tusd = new DepositWithdraw
                    {
                        Amount = Convert.ToDecimal(withdrawAmount),
                        UserId = ud.UserId,
                        CreatedDate = dt,
                        UsdAction = TransferUsdStatusEnum.Withdraw.ToString(),
                        Status = ActivityLogEnum.Pending.ToString()
                    }; //to user

                    _context.DepositWithdraw.Add(tusd);
                    _context.SaveChanges();

                    var activityLog = new ActivityLog
                    {
                        Userid = tusd.UserId,
                        ActivityType = ActivityLogEnum.WithdrawUsd.ToString(),
                        ActivityDate = dt,
                        Activity = "Requested to withdraw " + withdrawAmount + " Usd to Admin"
                    }; //to user
                    _context.ActivityLog.Add(activityLog);
                    _context.SaveChanges();

                    var mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = activityLog.Activity,
                        SmsMessageBody = activityLog.Activity,
                        MobileNumber = ud.Mobile,
                        To = ud.Email,
                        Subject = "Withdraw Usd"
                    }; //sending message to himself
                    _message.SendMessage(mm);

                    var au = _userManager.GetUsersInRoleAsync("Admin").Result; //sending message to requested person
                    mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = ud.UserId + " has requested " + withdrawAmount + " Usd to withdraw",
                        SmsMessageBody = ud.UserId + " has requested " + withdrawAmount + " Usd to withdraw",
                        MobileNumber = au[0].Mobile,
                        To = au[0].Email,
                        Subject = "Usd Withdraw Request"
                    };
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog
                    {
                        Userid = au[0].UserId,
                        ActivityType = ActivityLogEnum.WithdrawUsd.ToString(),
                        ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                        Activity = ud.UserId + " has requested " + withdrawAmount + " Usd to withdraw"
                    };
                    _context.ActivityLog.Add(activityLog);
                    _context.SaveChanges();


                    TempData["data"] = "Withdraw request for " + withdrawAmount + " Usd has been raised to admin";
                    return RedirectToAction("Usd", route);
                }

                TempData["error"] = "Insufficient amount in account to withdraw";
                return RedirectToAction("Usd", route);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData["error"] = "Error occurred while withdraw Usd";
            }

            return View("Usd");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/depositusd")]
        [Route("/investor/depositusd")]
        [Route("/client/depositusd")]
        [Route("/partner/depositusd")]
        public async Task<IActionResult> DepositUsd(string txtDepositTransactionId, string txtDepositAmount)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                var ud = await _userManager.GetUserAsync(User);

                var dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

                var tusd = new DepositWithdraw
                {
                    Amount = Convert.ToDecimal(txtDepositAmount),
                    UserId = ud.UserId,
                    CreatedDate = dt,
                    ModifiedDate = null,
                    TransactionId = txtDepositTransactionId,
                    UsdAction = TransferUsdStatusEnum.Deposit.ToString(),
                    Status = ActivityLogEnum.Pending.ToString()
                }; //to user

                _context.DepositWithdraw.Add(tusd);
                _context.SaveChanges();

                var activityLog = new ActivityLog
                {
                    Userid = tusd.UserId,
                    ActivityType = ActivityLogEnum.DepositUsd.ToString(),
                    ActivityDate = dt,
                    Activity = "Requested to deposit " + txtDepositAmount + " Usd to Admin"
                }; //to user
                _context.ActivityLog.Add(activityLog);
                _context.SaveChanges();

                var mm = new Message(_messageLogger)
                {
                    EmailMessageBody = activityLog.Activity,
                    SmsMessageBody = activityLog.Activity,
                    MobileNumber = ud.Mobile,
                    To = ud.Email,
                    Subject = "Deposit Usd"
                }; //sending message to himself
                _message.SendMessage(mm);

                var au = _userManager.GetUsersInRoleAsync("Admin").Result; //sending message to requested person
                mm = new Message(_messageLogger)
                {
                    EmailMessageBody = ud.UserId + " has requested " + txtDepositAmount + " Usd to deposit",
                    SmsMessageBody = ud.UserId + " has requested " + txtDepositAmount + " Usd to deposit",
                    MobileNumber = au[0].Mobile,
                    To = au[0].Email,
                    Subject = "Usd Deposit Request"
                };
                _message.SendMessage(mm);

                activityLog = new ActivityLog
                {
                    Userid = au[0].UserId,
                    ActivityType = ActivityLogEnum.DepositUsd.ToString(),
                    ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                    Activity = ud.UserId + " has requested " + txtDepositAmount + " Usd to deposit"
                };
                _context.ActivityLog.Add(activityLog);
                _context.SaveChanges();


                TempData["data"] = "Deposit request for " + txtDepositAmount + " Usd has been raised to admin";
                return RedirectToAction("Usd", route);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData["error"] = "Error occurred while deposit Usd";
            }

            return View("Usd");
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length) result.Append(unformattedKey.Substring(currentPosition));

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Bint"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableAuthenticatorViewModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        public async Task<ActionResult> GetAlerts()
        {
            var bd = new UsdDashboard();
            var dbf = new DbFunc(_logger);
            var r = _userManager.GetUserAsync(User).Result;
            bd.Stats = dbf.GetAlertStats(r.UserId);
            var ll = new Dictionary<string, string>();


            if (await _userManager.IsInRoleAsync(r, "Admin"))
            {
                ll.Add("USDRequested", bd.Stats.Rows[0][1].ToString() + " Usd requested");
                ll.Add("USDDepositRequest", bd.Stats.Rows[0][2].ToString() + " Usd deposit requests received");
                ll.Add("USDWithdrawRequest", bd.Stats.Rows[0][3].ToString() + " Usd withdraw requests received");
                ll.Add("AdminUsers", bd.Stats.Rows[0][7].ToString());
                ll.Add("InvestorUsers", bd.Stats.Rows[0][8].ToString());
                ll.Add("PartnerUsers", bd.Stats.Rows[0][9].ToString());
                ll.Add("ClientUsers", bd.Stats.Rows[0][10].ToString());
                ll.Add("LockedUsers", bd.Stats.Rows[0][11].ToString());
            }
            else
            {
                ll.Add("USDRequested", bd.Stats.Rows[0][1].ToString() + " Usd requested");
                ll.Add("USDDepositRequest", bd.Stats.Rows[0][4].ToString() + " Usd deposit requests pending");
                ll.Add("USDWithdrawRequest", bd.Stats.Rows[0][5].ToString() + " Usd withdraw requests pending");
                ll.Add("UsersCreated", bd.Stats.Rows[0][6].ToString());
            }


            return Json(ll);
        }

        #endregion
    }
}