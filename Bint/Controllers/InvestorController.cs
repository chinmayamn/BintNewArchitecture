using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bint.Data;
using Bint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace Bint.Controllers
{
    [Authorize(Roles = "Investor")]
    public class InvestorController : Controller
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ILogger<InvestorController> _logger;
        private UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        DBFunc dbf;
        public InvestorController(ILogger<InvestorController> logger, UserManager<ApplicationUser> usermanager, ApplicationDbContext context)
        {
            _logger = logger; _userManager = usermanager; _context = context;
             dbf = new DBFunc(_logger);
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
        public IActionResult Dashboard()
        {
            try
            {
                InvestorDashboard idb = new InvestorDashboard();
                UserCount u = new UserCount();

                IEnumerable<ApplicationUser> z;
                string id = _userManager.GetUserId(User);
                z = _userManager.Users.Where(x => x.Created_by == _userManager.GetUserAsync(User).Result.UserId);
                u.PartnerCount = z.Count();
                u.PartnerList = z.TakeLast(8);
                idb.TotalBGC = z.Sum(x => x.BGC);
                idb.TotalUSD = z.Sum(x => x.USD);
                idb.userCount = u;
                Payback pdb = new Payback();
                pdb.USDPaybackUser = dbf.GetUSDPaybackUser(_userManager.GetUserAsync(User).Result.UserId);
                idb._payback = pdb;
                return View(idb);
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
        public IActionResult Investments()
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
        public IActionResult Partners()
        {
            try
            {
                ViewBag.ReturnUrl = "/investor/partners";
                CustomerUserCreate m = new CustomerUserCreate();
                IEnumerable<ApplicationUser> z;
                string id = _userManager.GetUserId(User);
                z = _userManager.Users.Where(x=>x.Created_by==_userManager.GetUserAsync(User).Result.UserId);
                m.appUser = z;
                return View(m);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        public IActionResult USD()
        {
            try
            {
                USDDashboard bd = new USDDashboard();
                var r = _userManager.GetUserAsync(User).Result;
                bd._requestUSD = dbf.GetRequestUSDReport(r.UserId);
                bd._transferUSD = dbf.GetTransferUSDReport(r.UserId);
                var urole = ControllerContext.ActionDescriptor.ControllerName;
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                bd._withdrawUSD = dbf.GetDepositWithdrawUSDRequests(r.UserId, "Withdraw");
                bd._depositUSD = dbf.GetDepositWithdrawUSDRequests(r.UserId, "Deposit");
                bd._Stats = dbf.GetAlertStats(r.UserId);
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
        public IActionResult BGC()
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
       
        public IActionResult PartnerDetail()
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
        public IActionResult Register()
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
        [HttpGet]
        public IActionResult DeleteUser()
        {
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
                activityLog.ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                activityLog.Activity = "Deleted user " + u.UserId;
                _context.activitylog.Add(activityLog);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error occurred";
                _logger.LogError("Error occurred {id}", id);
            }
            return RedirectToAction("partners");
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
       

    }
}