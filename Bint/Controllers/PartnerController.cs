using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bint.Data;
using Bint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bint.Controllers
{
    [Authorize(Roles = "Partner")]
    public class PartnerController : Controller
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        private readonly ApplicationDbContext _context;
        private readonly ILogger<PartnerController> _logger;
        private readonly IHttpContextAccessor _request;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DBFunc _dbf;

        public PartnerController(IHttpContextAccessor httpContext, ILogger<PartnerController> logger,
            RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _request = httpContext;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _dbf = new DBFunc(logger);
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
                var pdbb = new PartnerDashboard();
                var pdb = new Payback
                {
                    UsdPaybackUser = _dbf.GetUSDPaybackUser(_userManager.GetUserAsync(User).Result.UserId)
                };
                pdbb.Payback = pdb;
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
                var ud = new UserProfileDoc();
                var id = _userManager.GetUserId(User);
                ud.UserDocs = _dbf.GetKYCDocs(id);
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

        public IActionResult UsdPayback()
        {
            try
            {
                var idb = new Payback
                {
                    UsdPayback = _dbf.GetUSDPayback(_userManager.GetUserAsync(User).Result.UserId)
                };
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
                UsdDashboard bd = new UsdDashboard();
                var r = _userManager.GetUserAsync(User).Result;
                bd.RequestUsd = _dbf.GetRequestUSDReport(r.UserId);
                bd.TransferUsd = _dbf.GetTransferUSDReport(r.UserId);
                bd.Stats = _dbf.GetAlertStats(r.UserId);
                var uRole = ControllerContext.ActionDescriptor.ControllerName;
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                bd.WithdrawUsd = _dbf.GetDepositWithdrawUSDRequests(r.UserId, "Withdraw");
                bd.DepositUsd = _dbf.GetDepositWithdrawUSDRequests(r.UserId, "Deposit");

                if (uRole == "Client")
                {
                    bd.QrCode = au[0].ClientQrCode;
                    bd.Tether = au[0].ClientTetherAddress;
                }
                else if (uRole == "Partner")
                {
                    bd.QrCode = au[0].PartnerQrCode;
                    bd.Tether = au[0].PartnerTetherAddress;
                }
                else if (uRole == "Investor")
                {
                    bd.QrCode = au[0].InvestorQrCode;
                    bd.Tether = au[0].InvestorTetherAddress;
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
                var m = new CustomerUserCreate();
                IOrderedEnumerable<ApplicationUser> z;
                z = _userManager.Users.AsEnumerable().Where(u => u.CreatedId == _userManager.GetUserId(User))
                    .OrderByDescending(x => x.CreatedOn.TimeOfDay);
                m.AppUser = z;
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

                var y = _userManager.GetUserAsync(User).Result;
                var activityLog = new ActivityLog
                {
                    Userid = y.UserId,
                    ActivityType = ActivityLogEnum.DeletePerson.ToString(),
                    ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone),
                    Activity = "Deleted user " + u.UserId
                };
                _context.ActivityLog.Add(activityLog);
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
                var act = new ActivityLogDashboard();
                act.ActivityLogTable = _dbf.GetUserActivityLog(r.UserId);

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
        public async Task<IActionResult> TetherUpdate(string txtTether, [FromForm] IFormFile formFile)
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
                    t = z + "\\wwwroot" + u.QrCode.Replace("/", "\\");
                    var fileInfo = new FileInfo(t);
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
                    u.QrCode = "/" + path.Replace("\\", "/").Replace("wwwroot/", "");
                    u.TetherAddress = txtTether;

                    var s = await _userManager.UpdateAsync(u);
                    if (s.Succeeded) return RedirectToAction("myprofile", route);

                    TempData["error"] = "Tether update failed";
                    _logger.LogError("Tether update failed", formFile);
                    return RedirectToAction("myprofile", route);
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