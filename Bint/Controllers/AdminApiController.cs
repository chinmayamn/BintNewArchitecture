using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bint.Models;
using Bint.Repository;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Bint.Data;

namespace Bint.Controllers
{
    [Produces("application/json")]
    [Route("api/Admin")]
    public class AdminApiController : Controller
    {
        private IAdminRepository _adminRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminApiController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Message> _messageLogger;
        private readonly IMessage _message;
        private readonly static TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public AdminApiController(RoleManager<IdentityRole> roleManager, IAdminRepository adminRepository, UserManager<ApplicationUser> userManager, ILogger<AdminApiController> logger, ApplicationDbContext context, ILogger<Message> messageLogger, IMessage message)
        {
            _adminRepository = adminRepository;
            _roleManager = roleManager; _userManager = userManager;_logger = logger;_context = context;
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
            return new string[] { "value1", "value2" };
        }

        // GET: api/Admin/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Admin
        [HttpPost]
        [Route("createrole")]
        public async Task<string> CreateRole([FromBody] string roleName)
        {
           
            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            return "success";
        }
        
        // PUT: api/Admin/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost]
        [Route("/admin/profilepic")]
        [Route("/investor/profilepic")]
        [Route("/client/profilepic")]
        [Route("/partner/profilepic")]
        public async Task<IActionResult> ProfilePic([FromForm]IFormFile formFile)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                string uniqueName = (DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString());
                string[] words = formFile.FileName.Split('.');
                string z1 = words[0] + uniqueName + "." + words[1];//file extension
                
                var path = Path.Combine("wwwroot", "uploads", z1);
                ApplicationUser u = _userManager.GetUserAsync(User).Result;

                //hard delete previous file
                try
                {
                    var z = Directory.GetCurrentDirectory();
                    var t = z+ "\\wwwroot"+ u.ProfilePicture.Replace("/", "\\");
                    var fileInfo = new System.IO.FileInfo(t);
                    if(fileInfo.Exists)
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
                    if(s.Succeeded)
                    {
                        return RedirectToAction("myprofile", route);
                    }
                    else
                    {
                        TempData["error"] = "Profile pic update failed";
                        _logger.LogError("Profile pic update failed", formFile);
                        return RedirectToAction("myprofile", route);
                    }
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
        public async Task<IActionResult> UploadDocs([FromForm]IFormFile formFile,string filename)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                string uniqueName = (DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString());
                string[] words = formFile.FileName.Split('.');
                string z1 = words[0] + uniqueName + "." + words[1];//file extension

                var path = Path.Combine("wwwroot", "docs", z1);
                ApplicationUser u = _userManager.GetUserAsync(User).Result;


                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream); //push file
               
                    Doc d = new Doc();
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
                    d.CreatedDate = indianTime;
                    d.DocPath = "/" + path.Replace("\\", "/").Replace("wwwroot/", ""); ;
                    d.Filename = filename;
                    d.Status = "Pending";
                    d.Userid = u.Id.ToString();
                    _context.Doc.Add(d);
                    _context.SaveChanges();
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
        public async Task<IActionResult> TetherUpdate(string txtTether,[FromForm]IFormFile formFile,string role)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                string uniqueName = (DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString());
                string[] words = formFile.FileName.Split('.');
                string z1 = words[0] + uniqueName + "." + words[1];//file extension

                var path = Path.Combine("wwwroot", "Tether", z1);
                ApplicationUser u = _userManager.GetUserAsync(User).Result;
                

                //hard delete previous file
                try
                {
                    var z = Directory.GetCurrentDirectory();var t = "";FileInfo fileInfo;
                    if(role=="Admin")
                    {
                         t = z + "\\wwwroot" + u.AdminQrCode.Replace("/", "\\");
                         fileInfo = new System.IO.FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                    else if(role=="Investor")
                    {
                         t = z + "\\wwwroot" + u.InvestorQrCode.Replace("/", "\\");
                         fileInfo = new System.IO.FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                    else if(role=="Partner")
                    {
                         t = z + "\\wwwroot" + u.PartnerQrCode.Replace("/", "\\");
                         fileInfo = new System.IO.FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                    else if(role=="Client")
                    {
                        t = z + "\\wwwroot" + u.ClientQrCode.Replace("/", "\\");
                        fileInfo = new System.IO.FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                    else
                    {

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
                        u.AdminQrCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");u.AdminTetherAddress = txtTether;
                    }
                    else if (role == "Investor")
                    {
                        u.InvestorQrCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", ""); u.InvestorTetherAddress= txtTether;
                    }
                    else if (role == "Partner")
                    {
                        u.PartnerQrCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", ""); u.PartnerTetherAddress = txtTether;
                    }
                    else if (role == "Client")
                    {
                        u.ClientQrCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", ""); u.ClientTetherAddress = txtTether;
                    }
                    else
                    {

                    }

                   
                    var s = await _userManager.UpdateAsync(u);
                    if (s.Succeeded)
                    {
                        return RedirectToAction("sitesettings", route);
                    }
                    else
                    {
                        TempData["error"] = "Tether update failed";
                        _logger.LogError("Tether update failed", formFile);
                        return RedirectToAction("sitesettings", route);
                    }
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
                DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                Message mm = new Message(_messageLogger);
                ActivityLog activityLog = new ActivityLog();
                DepositWithdraw tusd = _context.depositwithdraw.First(x => x.Id == id);  //deposit here

                if (status == "Deposit")
                {
                    tusd.ModifiedDate = dt;
                    tusd.Status = TransferUsdStatusEnum.Accepted.ToString();
                    _context.SaveChanges();

                    var ud = _userManager.Users.First(x=>x.UserId==tusd.UserId); //update his account with deposit
                    ud.Usd = ud.Usd + tusd.Amount;
                    await _userManager.UpdateAsync(ud);

                    TransferUsd myt = new TransferUsd
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
                    _context.transferusd.Add(myt);
                    _context.SaveChanges();


                    //to admin
                    activityLog.Userid = au[0].UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.ConfirmDepositUsd.ToString();
                    activityLog.Activity = "Confirmed deposit " + tusd.Amount + " Usd of user " + tusd.UserId + ". Balance : " + ud.Usd;
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = activityLog.Activity,
                        SmsMessageBody = activityLog.Activity,
                        MobileNumber = au[0].Mobile,
                        To = au[0].Email,
                        Subject = "Usd Deposit Confirmed"
                    };  //to admin
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog
                    {
                        Userid = tusd.UserId,
                        ActivityDate = dt,
                        ActivityType = ActivityLogEnum.ConfirmDepositUsd.ToString(),
                        Activity = "Confirmed deposit " + tusd.Amount + " Usd by Admin. Balance : " + ud.Usd
                    };  //to user
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    mm = new Message(_messageLogger); //to user
                    mm.EmailMessageBody = activityLog.Activity;
                    mm.SmsMessageBody = activityLog.Activity;
                    mm.MobileNumber = ud.Mobile;
                    mm.To = ud.Email;
                    mm.Subject = "Usd Deposit Confirmed";
                    _message.SendMessage(mm);

                    TempData["data"] = activityLog.Activity;
                    return Json("success");
                }

                if(status=="Reject")
                {
                    tusd.ModifiedDate = dt;
                    tusd.Status = TransferUsdStatusEnum.Rejected.ToString();
                    _context.SaveChanges();

                    var ud = await _userManager.GetUserAsync(User); //update his account with deposit


                    //to admin
                    activityLog.Userid = au[0].UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.Reject.ToString();
                    activityLog.Activity = "Rejected deposit " + tusd.Amount + " Usd of user " + tusd.UserId;
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    mm = new Message(_messageLogger);  //to admin
                    mm.EmailMessageBody = activityLog.Activity;
                    mm.SmsMessageBody = activityLog.Activity;
                    mm.MobileNumber = au[0].Mobile;
                    mm.To = au[0].Email;
                    mm.Subject = "Usd Deposit Rejected";
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog
                    {
                        Userid = tusd.UserId,
                        ActivityDate = dt,
                        ActivityType = ActivityLogEnum.Reject.ToString(),
                        Activity = "Rejected deposit " + tusd.Amount + " Usd by Admin"
                    };  //to user
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

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
                _logger.LogError(ex.ToString(), "Deposit confirm error{id} {status}", id,status);
                TempData["error"] = "Deposit confirm error";
                return Json("error");
            }
        }

        [HttpGet]
        [Route("/admin/confirmwithdraw")]
        public async Task<IActionResult> ConfirmWithdraw(int id, string status,string trnsid)
        {
            try
            {
                DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                Message mm = new Message(_messageLogger);
                ActivityLog activityLog = new ActivityLog();
                DepositWithdraw tusd = _context.depositwithdraw.First(x => x.Id == id);  //deposit here
                var ud = _userManager.Users.First(x => x.UserId == tusd.UserId);

                if (status == "Withdraw")
                {
                    if (ud.Usd >= tusd.Amount)
                    {
                        tusd.ModifiedDate = dt;
                        tusd.Status = TransferUsdStatusEnum.Accepted.ToString();
                        tusd.TransactionId = trnsid;
                        _context.SaveChanges();

                        //update his account with deposit
                        ud.Usd = ud.Usd - tusd.Amount;
                        await _userManager.UpdateAsync(ud);

                        TransferUsd myt = new TransferUsd
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
                        _context.transferusd.Add(myt);
                        _context.SaveChanges();


                        //to admin
                        activityLog.Userid = au[0].UserId;
                        activityLog.ActivityDate = dt;
                        activityLog.ActivityType = ActivityLogEnum.ConfirmWithdrawUsd.ToString();
                        activityLog.Activity = "Confirmed withdraw " + tusd.Amount + " Usd of user " + tusd.UserId + ". Balance : " + ud.Usd;
                        _context.activitylog.Add(activityLog);
                        _context.SaveChanges();

                        mm = new Message(_messageLogger);  //to admin
                        mm.EmailMessageBody = activityLog.Activity;
                        mm.SmsMessageBody = activityLog.Activity;
                        mm.MobileNumber = au[0].Mobile;
                        mm.To = au[0].Email;
                        mm.Subject = "Usd Withdraw Confirmed";
                        _message.SendMessage(mm);

                        activityLog = new ActivityLog();  //to user
                        activityLog.Userid = tusd.UserId;
                        activityLog.ActivityDate = dt;
                        activityLog.ActivityType = ActivityLogEnum.ConfirmWithdrawUsd.ToString();
                        activityLog.Activity = "Confirmed withdraw " + tusd.Amount + " Usd by Admin. Balance : " + ud.Usd;
                        _context.activitylog.Add(activityLog);
                        _context.SaveChanges();

                        mm = new Message(_messageLogger); //to user
                        mm.EmailMessageBody = activityLog.Activity;
                        mm.SmsMessageBody = activityLog.Activity;
                        mm.MobileNumber = ud.Mobile;
                        mm.To = ud.Email;
                        mm.Subject = "Usd Withdraw Confirmed";
                        _message.SendMessage(mm);

                        TempData["data"] = activityLog.Activity;
                        return Json("success");
                    }
                    else
                    {
                        TempData["error"] = "Insufficient amount in user account balance to withdraw";

                    }
                }
               

                if (status == "Reject")
                {
                    tusd.ModifiedDate = dt;
                    tusd.Status = TransferUsdStatusEnum.Rejected.ToString();
                    _context.SaveChanges();

                    var sud = await _userManager.GetUserAsync(User); //update his account with deposit


                    //to admin
                    activityLog.Userid = au[0].UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.Reject.ToString();
                    activityLog.Activity = "Rejected withdraw " + tusd.Amount + " Usd of user " + tusd.UserId;
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    mm = new Message(_messageLogger)
                    {
                        EmailMessageBody = activityLog.Activity,
                        SmsMessageBody = activityLog.Activity,
                        MobileNumber = au[0].Mobile,
                        To = au[0].Email,
                        Subject = "Usd Withdraw Rejected"
                    };  //to admin
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog
                    {
                        Userid = tusd.UserId,
                        ActivityDate = dt,
                        ActivityType = ActivityLogEnum.Reject.ToString(),
                        Activity = "Rejected withdraw " + tusd.Amount + " Usd by Admin"
                    };  //to user
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

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
