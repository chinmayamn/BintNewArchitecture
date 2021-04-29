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
        RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminApiController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Message> _messageLogger;
        private readonly IMessage _message;
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public AdminApiController(RoleManager<IdentityRole> roleManager, IAdminRepository adminRepository, UserManager<ApplicationUser> usermanager, ILogger<AdminApiController> logger, ApplicationDbContext context, ILogger<Message> messageLogger, IMessage message)
        {
            _adminRepository = adminRepository;
            _roleManager = roleManager; _userManager = usermanager;_logger = logger;_context = context;
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
        public async Task<string> CreateRole([FromBody] string rolename)
        {
           
            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(rolename));
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
                string UniqueName = (DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString());
                string[] words = formFile.FileName.Split('.');
                string z1 = words[0] + UniqueName + "." + words[1];//file extension
                
                var path = Path.Combine("wwwroot", "uploads", z1);
                ApplicationUser u = _userManager.GetUserAsync(User).Result;

                //hard delete previous file
                try
                {
                    var z = Directory.GetCurrentDirectory();
                    var t = z+ "\\wwwroot"+ u.Profilepicture.Replace("/", "\\");
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
                    u.Profilepicture = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");
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
                string UniqueName = (DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString());
                string[] words = formFile.FileName.Split('.');
                string z1 = words[0] + UniqueName + "." + words[1];//file extension

                var path = Path.Combine("wwwroot", "docs", z1);
                ApplicationUser u = _userManager.GetUserAsync(User).Result;


                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream); //push file
               
                    Doc d = new Doc();
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    d.CreatedDate = indianTime;
                    d.Docpath = "/" + path.Replace("\\", "/").Replace("wwwroot/", ""); ;
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
                string UniqueName = (DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString());
                string[] words = formFile.FileName.Split('.');
                string z1 = words[0] + UniqueName + "." + words[1];//file extension

                var path = Path.Combine("wwwroot", "Tether", z1);
                ApplicationUser u = _userManager.GetUserAsync(User).Result;
                

                //hard delete previous file
                try
                {
                    var z = Directory.GetCurrentDirectory();var t = "";FileInfo fileInfo;
                    if(role=="Admin")
                    {
                         t = z + "\\wwwroot" + u.AdminQRCode.Replace("/", "\\");
                         fileInfo = new System.IO.FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                    else if(role=="Investor")
                    {
                         t = z + "\\wwwroot" + u.InvestorQRCode.Replace("/", "\\");
                         fileInfo = new System.IO.FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                    else if(role=="Partner")
                    {
                         t = z + "\\wwwroot" + u.PartnerQRCode.Replace("/", "\\");
                         fileInfo = new System.IO.FileInfo(t);
                        if (fileInfo.Exists)
                            fileInfo.Delete();
                    }
                    else if(role=="Client")
                    {
                        t = z + "\\wwwroot" + u.ClientQRCode.Replace("/", "\\");
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
                        u.AdminQRCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");u.AdminTetherAddress = txtTether;
                    }
                    else if (role == "Investor")
                    {
                        u.InvestorQRCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", ""); u.InvestorTetherAddress= txtTether;
                    }
                    else if (role == "Partner")
                    {
                        u.PartnerQRCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", ""); u.PartnerTetherAddress = txtTether;
                    }
                    else if (role == "Client")
                    {
                        u.ClientQRCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", ""); u.ClientTetherAddress = txtTether;
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
                DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                Message mm = new Message(_messageLogger);
                ActivityLog activityLog = new ActivityLog();
                DepositWithdraw tusd = _context.depositwithdraw.First(x => x.Id == id);  //deposit here

                if (status == "Deposit")
                {
                    tusd.ModifiedDate = dt;
                    tusd.Status = TransferUSDStatusEnum.Accepted.ToString();
                    _context.SaveChanges();

                    var ud = _userManager.Users.First(x=>x.UserId==tusd.UserId); //update his account with deposit
                    ud.USD = ud.USD + tusd.Amount;
                    await _userManager.UpdateAsync(ud);

                    TransferUSD myt = new TransferUSD();
                    myt.Amount = tusd.Amount;
                    myt.FromStatus = TransferUSDStatusEnum.Deposit.ToString();
                    myt.ToStatus= TransferUSDStatusEnum.Deposit.ToString();
                    myt.FromTotalAmount = ud.USD;
                    myt.ToTotalAmount = ud.USD;
                    myt.RequestedDate = dt;
                    myt.TransferDate = dt;
                    myt.FromUserId = myt.ToUserId = tusd.UserId;
                    myt.Userid = ud.Id;
                    _context.transferusd.Add(myt);
                    _context.SaveChanges();


                    //to admin
                    activityLog.Userid = au[0].UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.ConfirmDepositUSD.ToString();
                    activityLog.Activity = "Confirmed deposit " + tusd.Amount + " USD of user " + tusd.UserId + ". Balance : " + ud.USD;
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    mm = new Message(_messageLogger);  //to admin
                    mm.EmailMessageBody = activityLog.Activity;
                    mm.SMSMessageBody = activityLog.Activity;
                    mm.MobileNumber = au[0].Mobile;
                    mm.To = au[0].Email;
                    mm.Subject = "USD Deposit Confirmed";
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog();  //to user
                    activityLog.Userid = tusd.UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.ConfirmDepositUSD.ToString();
                    activityLog.Activity = "Confirmed deposit " + tusd.Amount + " USD by Admin. Balance : " + ud.USD;
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    mm = new Message(_messageLogger); //to user
                    mm.EmailMessageBody = activityLog.Activity;
                    mm.SMSMessageBody = activityLog.Activity;
                    mm.MobileNumber = ud.Mobile;
                    mm.To = ud.Email;
                    mm.Subject = "USD Deposit Confirmed";
                    _message.SendMessage(mm);

                    TempData["data"] = activityLog.Activity;
                    return Json("success");
                }

                if(status=="Reject")
                {
                    tusd.ModifiedDate = dt;
                    tusd.Status = TransferUSDStatusEnum.Rejected.ToString();
                    _context.SaveChanges();

                    var ud = await _userManager.GetUserAsync(User); //update his account with deposit


                    //to admin
                    activityLog.Userid = au[0].UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.Reject.ToString();
                    activityLog.Activity = "Rejected deposit " + tusd.Amount + " USD of user " + tusd.UserId;
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    mm = new Message(_messageLogger);  //to admin
                    mm.EmailMessageBody = activityLog.Activity;
                    mm.SMSMessageBody = activityLog.Activity;
                    mm.MobileNumber = au[0].Mobile;
                    mm.To = au[0].Email;
                    mm.Subject = "USD Deposit Rejected";
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog();  //to user
                    activityLog.Userid = tusd.UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.Reject.ToString();
                    activityLog.Activity = "Rejected deposit " + tusd.Amount + " USD by Admin";
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    mm = new Message(_messageLogger); //to user
                    mm.EmailMessageBody = activityLog.Activity;
                    mm.SMSMessageBody = activityLog.Activity;
                    mm.MobileNumber = ud.Mobile;
                    mm.To = ud.Email;
                    mm.Subject = "USD Deposit Rejected";
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
                DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                Message mm = new Message(_messageLogger);
                ActivityLog activityLog = new ActivityLog();
                DepositWithdraw tusd = _context.depositwithdraw.First(x => x.Id == id);  //deposit here
                var ud = _userManager.Users.First(x => x.UserId == tusd.UserId);

                if (status == "Withdraw")
                {
                    if (ud.USD >= tusd.Amount)
                    {
                        tusd.ModifiedDate = dt;
                        tusd.Status = TransferUSDStatusEnum.Accepted.ToString();
                        tusd.TransactionId = trnsid;
                        _context.SaveChanges();

                        //update his account with deposit
                        ud.USD = ud.USD - tusd.Amount;
                        await _userManager.UpdateAsync(ud);

                        TransferUSD myt = new TransferUSD();
                        myt.Amount = tusd.Amount;
                        myt.FromStatus = TransferUSDStatusEnum.Withdraw.ToString();
                        myt.ToStatus = TransferUSDStatusEnum.Withdraw.ToString();
                        myt.FromTotalAmount = ud.USD;
                        myt.ToTotalAmount = ud.USD;
                        myt.RequestedDate = dt;
                        myt.TransferDate = dt;
                        myt.FromUserId = myt.ToUserId = tusd.UserId;
                        myt.Userid = ud.Id;
                        _context.transferusd.Add(myt);
                        _context.SaveChanges();


                        //to admin
                        activityLog.Userid = au[0].UserId;
                        activityLog.ActivityDate = dt;
                        activityLog.ActivityType = ActivityLogEnum.ConfirmWithdrawUSD.ToString();
                        activityLog.Activity = "Confirmed withdraw " + tusd.Amount + " USD of user " + tusd.UserId + ". Balance : " + ud.USD;
                        _context.activitylog.Add(activityLog);
                        _context.SaveChanges();

                        mm = new Message(_messageLogger);  //to admin
                        mm.EmailMessageBody = activityLog.Activity;
                        mm.SMSMessageBody = activityLog.Activity;
                        mm.MobileNumber = au[0].Mobile;
                        mm.To = au[0].Email;
                        mm.Subject = "USD Withdraw Confirmed";
                        _message.SendMessage(mm);

                        activityLog = new ActivityLog();  //to user
                        activityLog.Userid = tusd.UserId;
                        activityLog.ActivityDate = dt;
                        activityLog.ActivityType = ActivityLogEnum.ConfirmWithdrawUSD.ToString();
                        activityLog.Activity = "Confirmed withdraw " + tusd.Amount + " USD by Admin. Balance : " + ud.USD;
                        _context.activitylog.Add(activityLog);
                        _context.SaveChanges();

                        mm = new Message(_messageLogger); //to user
                        mm.EmailMessageBody = activityLog.Activity;
                        mm.SMSMessageBody = activityLog.Activity;
                        mm.MobileNumber = ud.Mobile;
                        mm.To = ud.Email;
                        mm.Subject = "USD Withdraw Confirmed";
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
                    tusd.Status = TransferUSDStatusEnum.Rejected.ToString();
                    _context.SaveChanges();

                    var sud = await _userManager.GetUserAsync(User); //update his account with deposit


                    //to admin
                    activityLog.Userid = au[0].UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.Reject.ToString();
                    activityLog.Activity = "Rejected withdraw " + tusd.Amount + " USD of user " + tusd.UserId;
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    mm = new Message(_messageLogger);  //to admin
                    mm.EmailMessageBody = activityLog.Activity;
                    mm.SMSMessageBody = activityLog.Activity;
                    mm.MobileNumber = au[0].Mobile;
                    mm.To = au[0].Email;
                    mm.Subject = "USD Withdraw Rejected";
                    _message.SendMessage(mm);

                    activityLog = new ActivityLog();  //to user
                    activityLog.Userid = tusd.UserId;
                    activityLog.ActivityDate = dt;
                    activityLog.ActivityType = ActivityLogEnum.Reject.ToString();
                    activityLog.Activity = "Rejected withdraw " + tusd.Amount + " USD by Admin";
                    _context.activitylog.Add(activityLog);
                    _context.SaveChanges();

                    mm = new Message(_messageLogger); //to user
                    mm.EmailMessageBody = activityLog.Activity;
                    mm.SMSMessageBody = activityLog.Activity;
                    mm.MobileNumber = sud.Mobile;
                    mm.To = sud.Email;
                    mm.Subject = "USD Withdraw Rejected";
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
