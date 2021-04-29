using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Bint.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Collections.Specialized;
using Exception = System.Exception;
using Microsoft.Extensions.Logging;
using Bint.Data;
using System.Threading.Tasks;
using System.Data;

namespace Bint.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IHttpContextAccessor _request;
        HttpClient _client= new HttpClient();
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment Environment;
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        DBFunc dbf;
        public AdminController(IHttpContextAccessor httpcontext, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> usermanager, IHostingEnvironment _environment, ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _request = httpcontext;
         
            string Baseurl = $"{_request.HttpContext.Request.Scheme}://{_request.HttpContext.Request.Host}";
            _client.BaseAddress = new Uri(Baseurl);
            _roleManager = roleManager; _userManager = usermanager;
            Environment = _environment;
            _logger = logger;
            _context = context;
            dbf = new DBFunc(_logger);
        }

   
        public IActionResult Index()
        {
           
            return View();
        }
        
        public IActionResult Dashboard()
        {
            try
            {
                AdminDashboard adb  = new AdminDashboard();
                UserCount u = new UserCount();

                u.AdminList = _userManager.GetUsersInRoleAsync("Admin").Result.TakeLast(8);
                u.InvestorList= _userManager.GetUsersInRoleAsync("Investor").Result.TakeLast(8);
                u.ClientList = _userManager.GetUsersInRoleAsync("Client").Result.TakeLast(8);
                u.PartnerList = _userManager.GetUsersInRoleAsync("Partner").Result.TakeLast(8);
                u.LockedUsersList = _userManager.Users.AsEnumerable().Where(uu => uu.LockoutEnd != null);
                u.PendingKYCCount = _userManager.Users.AsEnumerable().Where(uu => uu.KYC == "Pending").Count();

                u.AdminCount = u.AdminList.Count();
                u.InvestorCount = u.InvestorList.Count();
                u.ClientCount = u.ClientList.Count();
                u.PartnerCount = u.PartnerList.Count();
                u.LockedUsersCount = u.LockedUsersList.Count();

                adb.userCount = u; // load viewmodel with full data
                adb.TotalBGC = _userManager.Users.Sum(x => x.BGC);
                adb.TotalUSD = _userManager.Users.Sum(x => x.USD);

                adb.AdminRequestDashboard = dbf.GetAdminRequestDashboard();
                DataTable USDInvestment = dbf.GetUSDInvestment();
                Dictionary<string, string> dd = new Dictionary<string, string>();
                dd.Add("adminusd", USDInvestment.Rows[0][0].ToString());
                dd.Add("investorusd", USDInvestment.Rows[0][1].ToString());
                dd.Add("partnerusd", USDInvestment.Rows[0][2].ToString());
                dd.Add("clientusd", USDInvestment.Rows[0][3].ToString());
                adb._USDInvestment = dd;
                adb._USDInvestmentMonthwise = dbf.GetUSDInvestmentMonthwise();
                return View(adb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

            }

            return View();
        }
        public IActionResult MyProfile()
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

        public IActionResult USDDepositWithdraw()
        {
            try
            {
                USDDashboard bd = new USDDashboard();
                var r = _userManager.GetUserAsync(User).Result;
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                bd._withdrawUSD = dbf.GetDepositWithdrawUSDRequestsadmin("Withdraw");
                bd._depositUSD = dbf.GetDepositWithdrawUSDRequestsadmin("Deposit");
                bd._Stats = dbf.GetAlertStats(r.UserId);
                return View(bd);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();

        }
        public IActionResult BGCDepositWithdraw()
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
        [HttpGet]
        [Route("admin/admin")]
        [Route("admin/investor")]
        [Route("admin/client")]
        [Route("admin/locked")]
        [Route("admin/partner")]
        public ActionResult Members()
        {
            try
            {

          
            var route = Request.Path.Value.Split("/")[2];
            IEnumerable<ApplicationUser> z;
            ViewBag.U = route;
            CustomerUserCreate m = new CustomerUserCreate();
            List<IdentityRole> k = _roleManager.Roles.ToList(); m.urole = k;

            if (route.ToLower() =="admin")
            {
                ViewBag.ReturnUrl = "/admin/admin";  z = _userManager.GetUsersInRoleAsync("Admin").Result; m.appUser = z; return View(m);

            }
            else if (route.ToLower() == "investor")
            {
                ViewBag.ReturnUrl = "/admin/investor"; z = _userManager.GetUsersInRoleAsync("Investor").Result; m.appUser = z; return View(m);

            }
            else if (route.ToLower() == "client")
            {
                ViewBag.ReturnUrl = "/admin/client"; z = _userManager.GetUsersInRoleAsync("Client").Result;  m.appUser = z; return View(m);

            }
            else if (route.ToLower() == "partner")
            {
                ViewBag.ReturnUrl = "/admin/partner"; z = _userManager.GetUsersInRoleAsync("Partner").Result; m.appUser = z; return View(m);
            }
            else if (route.ToLower() == "locked")
            {
                ViewBag.ReturnUrl = "/admin/locked"; z = _userManager.Users.AsEnumerable().Where(u => u.LockoutEnd != null); m.appUser = z; return View(m);
            }
            else
            {

            }
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
        [Route("/admin/deleteuser")]
        [Route("/investor/deleteuser")]
        [Route("/client/deleteuser")]
        [Route("/partner/deleteuser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var route = Request.Path.Value.Split("/")[1]; //get current user
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
                activityLog.Activity = "Deleted user " +u.UserId;
                _context.activitylog.Add(activityLog);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                TempData["error"] = "Error occurred";
                _logger.LogError("Error occurred {id}", id);
            }
            return RedirectToAction(route, "Admin");
        }

      
        public ActionResult ProjectUseCases()
        {
            return View();
        }

        [HttpGet]
        public JsonResult setkyc(string userid, string kyc)
        {
            try
            {
                ApplicationUser u = _userManager.FindByIdAsync(userid).Result;
                u.KYC = kyc;
                var t =  _userManager.UpdateAsync(u).Result;
                if (t.Succeeded)
                    return Json(kyc);
                else
                    return Json("error");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json("error");
            }
        }
        public ActionResult SiteSettings()
        {
            try
            {
                SiteSettingDashboard sd = new SiteSettingDashboard();
                sd.SMSBalance = checkBalance();
                sd._regId = _context.regId.First();
                sd.u = _userManager.GetUserAsync(User).Result;

                return View(sd);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return View();
        }

        [HttpGet]
        public ActionResult Plans()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ; _logger.LogError(ex.ToString());
            }

            return View();
        }

        public string checkBalance()
        {
            try
            {
                using (var wb = new WebClient())
                {
                    byte[] response = wb.UploadValues("https://api.textlocal.in/balance/", new NameValueCollection()
                    {
                        {"apikey", "HSmxGKHOCC4-wJfRLr2vnPYhHv97HS7tsZbYpaOLq2"}
                    });

                    string result = System.Text.Encoding.UTF8.GetString(response);
                    object res = JsonConvert.DeserializeObject(result);
                    string balance = ((Newtonsoft.Json.Linq.JContainer) ((Newtonsoft.Json.Linq.JContainer)
                                ((Newtonsoft.Json.Linq.JContainer) ((Newtonsoft.Json.Linq.JContainer) res).First).First)
                            .First)
                        .First.ToString();
                    return balance;

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return "";

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

        public IActionResult USD()
        {
            try
            {
                USDDashboard bd = new USDDashboard();
                var r = _userManager.GetUserAsync(User).Result;
                bd._requestUSD = dbf.GetRequestUSDReport(r.UserId);
                bd._transferUSD = dbf.GetTransferUSDReport(r.UserId);
                bd._qrcode = r.AdminQRCode;
                bd._tether = r.AdminTetherAddress;
                bd._Stats = dbf.GetAlertStats(r.UserId);
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

       
   

    }
}