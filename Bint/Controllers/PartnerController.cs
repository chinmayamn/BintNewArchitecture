using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bint.Data;
using Bint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Logging;
namespace Bint.Controllers
{
    [Authorize(Roles = "Partner")]
    public class PartnerController : Controller
    {
        private static TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private IHttpContextAccessor _request;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PartnerController> _logger;
       
        private readonly ApplicationDbContext _context;
        DBFunc dbf;
        public PartnerController(IHttpContextAccessor httpcontext,ILogger<PartnerController> logger, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> usermanager, ApplicationDbContext context)
        {
            _request = httpcontext;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = usermanager;
            _context = context;
         
            dbf = new DBFunc(logger);
        }
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        public ActionResult Stats()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        public IActionResult Dashboard()
        {
            try
            {
                PartnerDashboard pdbb = new PartnerDashboard();
                Payback pdb = new Payback();
                pdb.USDPaybackUser = dbf.GetUSDPaybackUser(_userManager.GetUserAsync(User).Result.UserId);
                pdbb._payback = pdb;
                return View(pdbb);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        public IActionResult MyProfile()
        {
            try
            {
                UserProfileDoc ud = new UserProfileDoc();
                var id = _userManager.GetUserId(User);
                ud.UserDocs = dbf.GetKYCDocs(id);
                return View(ud);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        public IActionResult Plans()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        public IActionResult USDPayback()
        {
            try
            {
                Payback idb = new Payback();
                idb.USDPayback = dbf.GetUSDPayback(_userManager.GetUserAsync(User).Result.UserId);
                return View(idb);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();

        }
        public IActionResult Usd()
        {
            try
            {
                USDDashboard bd = new USDDashboard();
                var r = _userManager.GetUserAsync(User).Result;
                bd._requestUSD = dbf.GetRequestUSDReport(r.UserId);
                bd._transferUSD = dbf.GetTransferUSDReport(r.UserId);
                bd._Stats = dbf.GetAlertStats(r.UserId);
                var urole = ControllerContext.ActionDescriptor.ControllerName;
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                bd._withdrawUSD = dbf.GetDepositWithdrawUSDRequests(r.UserId, "Withdraw");
                bd._depositUSD = dbf.GetDepositWithdrawUSDRequests(r.UserId, "Deposit");

                if (urole == "Client")
                {
                    bd._qrcode = au[0].ClientQRCode;
                    bd._tether = au[0].ClientTetherAddress;
                }
                else if (urole == "Partner")
                {
                    bd._qrcode = au[0].PartnerQRCode;
                    bd._tether = au[0].PartnerTetherAddress;
                }
                else if (urole == "Investor")
                {
                    bd._qrcode = au[0].InvestorQRCode;
                    bd._tether = au[0].InvestorTetherAddress;
                }
                else
                {


                }
                return View(bd);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        public IActionResult Bgc()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        
        public IActionResult Clients()
        {
            try
            {
                ViewBag.ReturnUrl = "/partner/clients";
                CustomerUserCreate m = new CustomerUserCreate();
                IOrderedEnumerable<ApplicationUser> z;
                z = _userManager.Users.AsEnumerable().Where(u => u.CreatedId == _userManager.GetUserId(User)).OrderByDescending(x => x.CreatedOn.TimeOfDay);
                m.appUser = z;
                return View(m);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var u = await _userManager.FindByIdAsync(id);
                var result = await _userManager.DeleteAsync(u);
                if (result.Succeeded)
                    TempData["data"] = "User deleted successfully";

                ApplicationUser y = _userManager.GetUserAsync(User).Result;
                ActivityLog activityLog = new ActivityLog();
                activityLog.Userid = y.UserId;
                activityLog.ActivityType = ActivityLogEnum.DeletePerson.ToString();
                activityLog.ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
                activityLog.Activity = "Deleted user " + u.UserId;
                _context.activitylog.Add(activityLog);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error occurred";
                _logger.LogError("Error occurred {id}", id);
            }
            return RedirectToAction("clients");
        }
        public IActionResult ActivityLog()
        {
            try
            {
                var r = _userManager.GetUserAsync(User).Result;
                ActivityLogDashboard act = new ActivityLogDashboard();
                act.activityLogTable = dbf.GetUserActivityLog(r.UserId);

                return View(act);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        [HttpPost]
        [Route("/partner/TetherUpdate")]
        public async Task<IActionResult> TetherUpdate(string txtTether, [FromForm]IFormFile formFile)
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
                    var z = Directory.GetCurrentDirectory();
                    var t = ""; FileInfo fileInfo;
                    t = z + "\\wwwroot" + u.QRCode.Replace("/", "\\");
                    fileInfo = new System.IO.FileInfo(t);
                    if (fileInfo.Exists)
                        fileInfo.Delete();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString(), "Previous tether hard delete error {u.Id}", u.Id);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                    u.QRCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");
                    u.TetherAddress = txtTether;

                    var s = await _userManager.UpdateAsync(u);
                    if (s.Succeeded)
                    {
                        return RedirectToAction("myprofile", route);
                    }
                    else
                    {
                        TempData["error"] = "Tether update failed";
                        _logger.LogError("Tether update failed", formFile);
                        return RedirectToAction("myprofile", route);
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
    }
}