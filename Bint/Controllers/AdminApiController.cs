using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bint.Data;
using Bint.Models;
using Bint.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bint.Controllers
{
    [Produces("application/json")]
    [Route("api/Admin")]
    public class AdminApiController : Controller
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminApiController> _logger;
        private readonly IMessage _message;
        private readonly ILogger<Message> _messageLogger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private IAdminRepository _adminRepository;

        public AdminApiController(RoleManager<IdentityRole> roleManager, IAdminRepository adminRepository,
            UserManager<ApplicationUser> userManager, ILogger<AdminApiController> logger, ApplicationDbContext context,
            ILogger<Message> messageLogger, IMessage message)
        {
            _adminRepository = adminRepository;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _messageLogger = messageLogger;
            _message = message;
        }

        [Route("GetUserRoles")]
        [HttpGet]
        public IEnumerable<IdentityRole> UserRoles()
        {
            return _roleManager.Roles.ToList();
        }

        // GET: api/Admin
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] {"value1", "value2"};
        }

        // GET: api/Admin/5
        [HttpGet("{id:int}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Admin
        [HttpPost]
        [Route("createrole")]
        public async Task<string> CreateRole([FromBody] string roleName)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            return "success";
        }

        // PUT: api/Admin/5
        [HttpPut("{id:int}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id:int}")]
        public void Delete(int id)
        {
        }

        [HttpPost]
        [Route("/admin/profilepic")]
        [Route("/investor/profilepic")]
        [Route("/client/profilepic")]
        [Route("/partner/profilepic")]
        public async Task<IActionResult> ProfilePic([FromForm] IFormFile formFile)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                var uniqueName = DateTime.Now.Year + DateTime.Now.Month.ToString() + DateTime.Now.Day +
                                 DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second +
                                 DateTime.Now.Millisecond;
                var words = formFile.FileName.Split('.');
                var z1 = words[0] + uniqueName + "." + words[1]; //file extension

                var path = Path.Combine("wwwroot", "uploads", z1);
                var u = _userManager.GetUserAsync(User).Result;

                //hard delete previous file
                try
                {
                    var z = Directory.GetCurrentDirectory();
                    var t = z + "\\wwwroot" + u.ProfilePicture.Replace("/", "\\");
                    var fileInfo = new FileInfo(t);
                    if (fileInfo.Exists)
                        fileInfo.Delete();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString(), "Profile pic hard delete error {u.Id}", u.Id);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                    u.ProfilePicture = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");
                    var s = await _userManager.UpdateAsync(u);
                    if (s.Succeeded) return RedirectToAction("myprofile", route);

                    TempData["error"] = "Profile pic update failed";
                    _logger.LogError("Profile pic update failed", formFile);
                    return RedirectToAction("myprofile", route);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "Profile pic change error", formFile);
                TempData["error"] = "Profile pic change error";
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("/admin/UploadDocs")]
        [Route("/investor/UploadDocs")]
        [Route("/client/UploadDocs")]
        [Route("/partner/UploadDocs")]
        public async Task<IActionResult> UploadDocs([FromForm] IFormFile formFile, string filename)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                var uniqueName = DateTime.Now.Year + DateTime.Now.Month.ToString() + DateTime.Now.Day +
                                 DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second +
                                 DateTime.Now.Millisecond;
                var words = formFile.FileName.Split('.');
                var z1 = words[0] + uniqueName + "." + words[1]; //file extension

                var path = Path.Combine("wwwroot", "docs", z1);
                var u = _userManager.GetUserAsync(User).Result;


                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream); //push file

                    var d = new Doc();
                    var indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
                    d.CreatedDate = indianTime;
                    d.DocPath = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");
                    d.Filename = filename;
                    d.Status = "Pending";
                    d.Userid = u.Id;
                    _context.Doc.Add(d);
                    await _context.SaveChangesAsync();
                    TempData["data"] = "File uploaded successfully";
                    return RedirectToAction("myprofile", route);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "Document upload error", formFile);
                TempData["error"] = "Document upload error";
                return BadRequest();
            }
        }


        [HttpPost]
        [Route("/admin/TetherUpdate")]
        public async Task<IActionResult> TetherUpdate(string txtTether, [FromForm] IFormFile formFile, string role)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                var uniqueName = DateTime.Now.Year + DateTime.Now.Month.ToString() + DateTime.Now.Day +
                                 DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second +
                                 DateTime.Now.Millisecond;
                var words = formFile.FileName.Split('.');
                var z1 = words[0] + uniqueName + "." + words[1]; //file extension

                var path = Path.Combine("wwwroot", "Tether", z1);
                var u = _userManager.GetUserAsync(User).Result;


                //hard delete previous file
                try
                {
                    var z = Directory.GetCurrentDirectory();
                    var t = "";
                    FileInfo fileInfo;
                    if (role == "Admin")
                    {
                        t = z + "\\wwwroot" + u.AdminQrCode.Replace("/", "\\");
                        fileInfo = new FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                    else if (role == "Investor")
                    {
                        t = z + "\\wwwroot" + u.InvestorQrCode.Replace("/", "\\");
                        fileInfo = new FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                    else if (role == "Partner")
                    {
                        t = z + "\\wwwroot" + u.PartnerQrCode.Replace("/", "\\");
                        fileInfo = new FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                    else if (role == "Client")
                    {
                        t = z + "\\wwwroot" + u.ClientQrCode.Replace("/", "\\");
                        fileInfo = new FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString(), "Previous tether hard delete error {u.Id}", u.Id);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                    if (role == "Admin")
                    {
                        u.AdminQrCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");
                        u.AdminTetherAddress = txtTether;
                    }
                    else if (role == "Investor")
                    {
                        u.InvestorQrCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");
                        u.InvestorTetherAddress = txtTether;
                    }
                    else if (role == "Partner")
                    {
                        u.PartnerQrCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");
                        u.PartnerTetherAddress = txtTether;
                    }
                    else if (role == "Client")
                    {
                        u.ClientQrCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");
                        u.ClientTetherAddress = txtTether;
                    }


                    var s = await _userManager.UpdateAsync(u);
                    if (s.Succeeded) return RedirectToAction("sitesettings", route);

                    TempData["error"] = "Tether update failed";
                    _logger.LogError("Tether update failed", formFile);
                    return RedirectToAction("sitesettings", route);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "Tether details change error", formFile);
                TempData["error"] = "Tether details change error";
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/admin/confirmdeposit")]
        public async Task<IActionResult> ConfirmDeposit(int id, string status)
        {
            try
            {
                var dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                var mm = new Message(_messageLogger);
                var activityLog = new ActivityLog();
                var tusd = _context.DepositWithdraw.First(x => x.Id == id); //deposit here

                if (status == "Deposit")
                {
                    tusd.ModifiedDate = dt;
                    tusd.Status = TransferUsdStatusEnum.Accepted.ToString();
                    await _context.SaveChangesAsync();

                    var ud = _userManager.Users.First(x => x.UserId == tusd.UserId); //update his account with deposit
                    ud.Usd += tusd.Amount;
                    await _userManager.UpdateAsync(ud);

                    var myt = new TransferUsd
                    {
                        Amount = tusd.Amount,
                        FromStatus = TransferUsdStatusEnum.Deposit.ToString(),
                        ToStatus = TransferUsdStatusEnum.Deposit.ToString(),
                        FromTotalAmount = ud.Usd,
                        ToTotalAmount = ud.Usd,
                        RequestedDate = dt,
                        TransferDate = dt,
                        FromUserId = tusd.UserId,
                        ToUserId = tusd.UserId,
                        Userid = ud.Id
                    };
                    _context.TransferUsd.Add(myt);
                    await _context.SaveChangesAsync();


                    //to admin
                    activityLog.Userid = au[0].UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.ConfirmDepositUsd.ToString();
                    activityLog.Activity = "Confirmed deposit " + tusd.Amount + " Usd of user " + tusd.UserId +
                                           ". Balance : " + ud.Usd;
                    _context.ActivityLog.Add(activityLog);
                    await _context.SaveChangesAsync();

                    mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = activityLog.Activity,
                        SmsMessageBody = activityLog.Activity,
                        MobileNumber = au[0].Mobile,
                        To = au[0].Email,
                        Subject = "Usd Deposit Confirmed"
                    }; //to admin
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog
                    {
                        Userid = tusd.UserId,
                        ActivityDate = dt,
                        ActivityType = ActivityLogEnum.ConfirmDepositUsd.ToString(),
                        Activity = "Confirmed deposit " + tusd.Amount + " Usd by Admin. Balance : " + ud.Usd
                    }; //to user
                    _context.ActivityLog.Add(activityLog);
                    await _context.SaveChangesAsync();

                    mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = activityLog.Activity,
                        SmsMessageBody = activityLog.Activity,
                        MobileNumber = ud.Mobile,
                        To = ud.Email,
                        Subject = "Usd Deposit Confirmed"
                    }; //to user
                    _message.SendMessage(mm);

                    TempData["data"] = activityLog.Activity;
                    return Json("success");
                }

                if (status == "Reject")
                {
                    tusd.ModifiedDate = dt;
                    tusd.Status = TransferUsdStatusEnum.Rejected.ToString();
                    await _context.SaveChangesAsync();

                    var ud = await _userManager.GetUserAsync(User); //update his account with deposit


                    //to admin
                    activityLog.Userid = au[0].UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.Reject.ToString();
                    activityLog.Activity = "Rejected deposit " + tusd.Amount + " Usd of user " + tusd.UserId;
                    _context.ActivityLog.Add(activityLog);
                    await _context.SaveChangesAsync();

                    mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = activityLog.Activity,
                        SmsMessageBody = activityLog.Activity,
                        MobileNumber = au[0].Mobile,
                        To = au[0].Email,
                        Subject = "Usd Deposit Rejected"
                    }; //to admin
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog
                    {
                        Userid = tusd.UserId,
                        ActivityDate = dt,
                        ActivityType = ActivityLogEnum.Reject.ToString(),
                        Activity = "Rejected deposit " + tusd.Amount + " Usd by Admin"
                    }; //to user
                    _context.ActivityLog.Add(activityLog);
                    await _context.SaveChangesAsync();

                    mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = activityLog.Activity,
                        SmsMessageBody = activityLog.Activity,
                        MobileNumber = ud.Mobile,
                        To = ud.Email,
                        Subject = "Usd Deposit Rejected"
                    }; //to user
                    _message.SendMessage(mm);

                    TempData["data"] = activityLog.Activity;
                    return Json("success");
                }

                return Json("success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "Deposit confirm error{id} {status}", id, status);
                TempData["error"] = "Deposit confirm error";
                return Json("error");
            }
        }

        [HttpGet]
        [Route("/admin/confirmwithdraw")]
        public async Task<IActionResult> ConfirmWithdraw(int id, string status, string trnsId)
        {
            try
            {
                var dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                var mm = new Message(_messageLogger);
                var activityLog = new ActivityLog();
                var tusd = _context.DepositWithdraw.First(x => x.Id == id); //deposit here
                var ud = _userManager.Users.First(x => x.UserId == tusd.UserId);

                if (status == "Withdraw")
                {
                    if (ud.Usd >= tusd.Amount)
                    {
                        tusd.ModifiedDate = dt;
                        tusd.Status = TransferUsdStatusEnum.Accepted.ToString();
                        tusd.TransactionId = trnsId;
                        await _context.SaveChangesAsync();

                        //update his account with deposit
                        ud.Usd -= tusd.Amount;
                        await _userManager.UpdateAsync(ud);

                        var myt = new TransferUsd
                        {
                            Amount = tusd.Amount,
                            FromStatus = TransferUsdStatusEnum.Withdraw.ToString(),
                            ToStatus = TransferUsdStatusEnum.Withdraw.ToString(),
                            FromTotalAmount = ud.Usd,
                            ToTotalAmount = ud.Usd,
                            RequestedDate = dt,
                            TransferDate = dt,
                            FromUserId = tusd.UserId,
                            ToUserId = tusd.UserId,
                            Userid = ud.Id
                        };
                        _context.TransferUsd.Add(myt);
                        await _context.SaveChangesAsync();


                        //to admin
                        activityLog.Userid = au[0].UserId;
                        activityLog.ActivityDate = dt;
                        activityLog.ActivityType = ActivityLogEnum.ConfirmWithdrawUsd.ToString();
                        activityLog.Activity = "Confirmed withdraw " + tusd.Amount + " Usd of user " + tusd.UserId +
                                               ". Balance : " + ud.Usd;
                        _context.ActivityLog.Add(activityLog);
                        await _context.SaveChangesAsync();

                        mm = new Message(_messageLogger)
                        {
                            EmailMessageBody = activityLog.Activity,
                            SmsMessageBody = activityLog.Activity,
                            MobileNumber = au[0].Mobile,
                            To = au[0].Email,
                            Subject = "Usd Withdraw Confirmed"
                        }; //to admin
                        _message.SendMessage(mm);

                        activityLog = new ActivityLog
                        {
                            Userid = tusd.UserId,
                            ActivityDate = dt,
                            ActivityType = ActivityLogEnum.ConfirmWithdrawUsd.ToString(),
                            Activity =
                            "Confirmed withdraw " + tusd.Amount + " Usd by Admin. Balance : " + ud.Usd
                        }; //to user
                        _context.ActivityLog.Add(activityLog);
                        await _context.SaveChangesAsync();

                        mm = new Message(_messageLogger)
                        {
                            EmailMessageBody = activityLog.Activity,
                            SmsMessageBody = activityLog.Activity,
                            MobileNumber = ud.Mobile,
                            To = ud.Email,
                            Subject = "Usd Withdraw Confirmed"
                        }; //to user
                        _message.SendMessage(mm);

                        TempData["data"] = activityLog.Activity;
                        return Json("success");
                    }

                    TempData["error"] = "Insufficient amount in user account balance to withdraw";
                }


                if (status == "Reject")
                {
                    tusd.ModifiedDate = dt;
                    tusd.Status = TransferUsdStatusEnum.Rejected.ToString();
                    await _context.SaveChangesAsync();

                    var sud = await _userManager.GetUserAsync(User); //update his account with deposit


                    //to admin
                    activityLog.Userid = au[0].UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.Reject.ToString();
                    activityLog.Activity = "Rejected withdraw " + tusd.Amount + " Usd of user " + tusd.UserId;
                    _context.ActivityLog.Add(activityLog);
                    await _context.SaveChangesAsync();

                    mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = activityLog.Activity,
                        SmsMessageBody = activityLog.Activity,
                        MobileNumber = au[0].Mobile,
                        To = au[0].Email,
                        Subject = "Usd Withdraw Rejected"
                    }; //to admin
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog
                    {
                        Userid = tusd.UserId,
                        ActivityDate = dt,
                        ActivityType = ActivityLogEnum.Reject.ToString(),
                        Activity = "Rejected withdraw " + tusd.Amount + " Usd by Admin"
                    }; //to user
                    _context.ActivityLog.Add(activityLog);
                    await _context.SaveChangesAsync();

                    mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = activityLog.Activity,
                        SmsMessageBody = activityLog.Activity,
                        MobileNumber = sud.Mobile,
                        To = sud.Email,
                        Subject = "Usd Withdraw Rejected"
                    }; //to user
                    _message.SendMessage(mm);

                    TempData["data"] = activityLog.Activity;
                    return Json("success");
                }

                return Json("success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "Withdraw confirm error{id} {status}", id, status);
                TempData["error"] = "Withdraw confirm error";
                return Json("error");
            }
        }
    }
}