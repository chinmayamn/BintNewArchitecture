using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bint.Data;
using Bint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bint.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly HttpClient _client = new HttpClient();
        private readonly IHttpContextAccessor _request;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbFunc _dbFunc;
        private readonly IHostingEnvironment _environment;

        public AdminController(IHttpContextAccessor httpContext, RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager, IHostingEnvironment environment, ILogger<AdminController> logger,
            ApplicationDbContext context, IDbFunc iDbFunc)
        {
            _request = httpContext;

            var baseUrl = $"{_request.HttpContext.Request.Scheme}://{_request.HttpContext.Request.Host}";
            _client.BaseAddress = new Uri(baseUrl);
            _roleManager = roleManager;
            _userManager = userManager;
            _environment = environment;
            _logger = logger;
            _context = context;
            _dbFunc = iDbFunc;
        }

        public IActionResult Dashboard()
        {
            try
            {
                var adb = new AdminDashboard();
                var u = new UserCount
                {
                    AdminList = _userManager.GetUsersInRoleAsync("Admin").Result.TakeLast(8),
                    InvestorList = _userManager.GetUsersInRoleAsync("Investor").Result.TakeLast(8),
                    ClientList = _userManager.GetUsersInRoleAsync("Client").Result.TakeLast(8),
                    PartnerList = _userManager.GetUsersInRoleAsync("Partner").Result.TakeLast(8),
                    LockedUsersList = _userManager.Users.AsEnumerable().Where(uu => uu.LockoutEnd != null),
                    PendingKycCount = _userManager.Users.AsEnumerable().Count(uu => uu.Kyc == "Pending")
                };

                u.AdminCount = u.AdminList.Count();
                u.InvestorCount = u.InvestorList.Count();
                u.ClientCount = u.ClientList.Count();
                u.PartnerCount = u.PartnerList.Count();
                u.LockedUsersCount = u.LockedUsersList.Count();

                adb.UserCount = u; // load viewmodel with full data
                adb.TotalBgc = _userManager.Users.Sum(x => x.Bgc);
                adb.TotalUsd = _userManager.Users.Sum(x => x.Usd);

                adb.AdminRequestDashboard = _dbFunc.GetAdminRequestDashboard();
                var usdInvestment = _dbFunc.GetUsdInvestment();
                Dictionary<string,string> dd = new Dictionary<string, string>
                {
                    {"adminusd", usdInvestment.Rows[0][0].ToString()},
                    {"investorusd", usdInvestment.Rows[0][1].ToString()},
                    {"partnerusd", usdInvestment.Rows[0][2].ToString()},
                    {"clientusd", usdInvestment.Rows[0][3].ToString()}
                };
                adb.UsdInvestment = dd;
                adb.UsdInvestmentMonthWise = _dbFunc.GetUsdInvestmentMonthwise();
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

        public IActionResult UsdDepositWithdraw()
        {
            try
            {
                var bd = new UsdDashboard();
                var r = _userManager.GetUserAsync(User).Result;
                bd.WithdrawUsd = _dbFunc.GetDepositWithdrawUsdRequestsadmin("Withdraw");
                bd.DepositUsd = _dbFunc.GetDepositWithdrawUsdRequestsadmin("Deposit");
                bd.Stats = _dbFunc.GetAlertStats(r.UserId);
                return View(bd);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return View();
        }

        public IActionResult BgcDepositWithdraw()
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

        public IActionResult UsdPayback()
        {
            try
            {
                var idb = new Payback {UsdPayback = _dbFunc.GetUsdPayback(_userManager.GetUserAsync(User).Result.UserId)};
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
                var m = new CustomerUserCreate();
                var k = _roleManager.Roles.ToList();
                m.URole = k;

                switch (route.ToLower())
                {
                    case "admin":
                        ViewBag.ReturnUrl = "/admin/admin";
                        z = _userManager.GetUsersInRoleAsync("Admin").Result;
                        m.AppUser = z;
                        return View(m);
                    case "investor":
                        ViewBag.ReturnUrl = "/admin/investor";
                        z = _userManager.GetUsersInRoleAsync("Investor").Result;
                        m.AppUser = z;
                        return View(m);
                    case "client":
                        ViewBag.ReturnUrl = "/admin/client";
                        z = _userManager.GetUsersInRoleAsync("Client").Result;
                        m.AppUser = z;
                        return View(m);
                    case "partner":
                        ViewBag.ReturnUrl = "/admin/partner";
                        z = _userManager.GetUsersInRoleAsync("Partner").Result;
                        m.AppUser = z;
                        return View(m);
                    case "locked":
                        ViewBag.ReturnUrl = "/admin/locked";
                        z = _userManager.Users.AsEnumerable().Where(u => u.LockoutEnd != null);
                        m.AppUser = z;
                        return View(m);
                    default:
                        return View();
                }
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

                var y = _userManager.GetUserAsync(User).Result;
                var activityLog = new ActivityLog
                {
                    Userid = y.UserId,
                    ActivityType = ActivityLogEnum.DeletePerson.ToString(),
                    ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                    Activity = "Deleted user " + u.UserId
                };
                _context.ActivityLog.Add(activityLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
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
        public JsonResult SetKyc(string userid, string kyc)
        {
            try
            {
                var u = _userManager.FindByIdAsync(userid).Result;
                u.Kyc = kyc;
                var t = _userManager.UpdateAsync(u).Result;
                if (t.Succeeded)
                    return Json(kyc);
                return Json("error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json("error");
            }
        }

        public ActionResult SiteSettings()
        {
            try
            {
                var sd = new SiteSettingDashboard
                {
                    SmsBalance = CheckBalance(),
                    RegId = _context.RegId.First(),
                    ApplicationUser = _userManager.GetUserAsync(User).Result
                };

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
                _logger.LogError(ex.ToString());
            }

            return View();
        }

        public string CheckBalance()
        {
            try
            {
                using (var wb = new WebClient())
                {
                    var response = wb.UploadValues("https://api.textlocal.in/balance/", new NameValueCollection
                    {
                        {"apikey", "HSmxGKHOCC4-wJfRLr2vnPYhHv97HS7tsZbYpaOLq2"}
                    });

                    var result = Encoding.UTF8.GetString(response);
                    var res = JsonConvert.DeserializeObject(result);
                    var balance = ((JContainer) ((JContainer)
                                ((JContainer) ((JContainer) res).First).First)
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
                var act = new ActivityLogDashboard
                {
                    ActivityLogTable = _dbFunc.GetUserActivityLog(r.UserId)
                };

                return View(act);
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
                var bd = new UsdDashboard();
                var r = _userManager.GetUserAsync(User).Result;
                bd.RequestUsd = _dbFunc.GetRequestUsdReport(r.UserId);
                bd.TransferUsd = _dbFunc.GetTransferUsdReport(r.UserId);
                bd.QrCode = r.AdminQrCode;
                bd.Tether = r.AdminTetherAddress;
                bd.Stats = _dbFunc.GetAlertStats(r.UserId);
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
    }
}