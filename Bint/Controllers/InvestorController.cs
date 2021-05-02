using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bint.Data;
using Bint.Models;
using Bint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bint.Controllers
{
    [Authorize(Roles = "Investor")]
    public class InvestorController : Controller
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly IApplicationDbContext _context;
        private readonly IDbFunc _dbFunc;
        private readonly ILogger<InvestorController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvestorController(ILogger<InvestorController> logger, UserManager<ApplicationUser> userManager,
            IApplicationDbContext context, IDbFunc iDbFunc)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _dbFunc = iDbFunc;
        }

        public IActionResult Dashboard()
        {
            try
            {
                var idb = new InvestorDashboard();
                var u = new UserCount();
                IEnumerable<ApplicationUser> z = _userManager.Users.Where(x => x.CreatedBy == _userManager.GetUserAsync(User).Result.UserId);
                u.PartnerCount = z.Count();
                u.PartnerList = z.TakeLast(8);
                idb.TotalBgc = z.Sum(x => x.Bgc);
                idb.TotalUsd = z.Sum(x => x.Usd);
                idb.UserCount = u;
                var pdb = new Payback
                {
                    UsdPaybackUser = _dbFunc.GetUsdPaybackUser(_userManager.GetUserAsync(User).Result.UserId)
                };
                idb.Payback = pdb;
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
                var ud = new UserProfileDoc();
                var id = _userManager.GetUserId(User);
                ud.UserDocs = _dbFunc.GetKycDocs(id);
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

        public IActionResult UsdPayback()
        {
            try
            {
                var idb = new Payback
                {
                    UsdPayback = _dbFunc.GetUsdPayback(_userManager.GetUserAsync(User).Result.UserId)
                };
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
                var m = new CustomerUserCreate();
                IEnumerable<ApplicationUser> z = _userManager.Users.Where(x => x.CreatedBy == _userManager.GetUserAsync(User).Result.UserId);
                m.AppUser = z;
                return View(m);
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
                var uRole = ControllerContext.ActionDescriptor.ControllerName;
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                bd.WithdrawUsd = _dbFunc.GetDepositWithdrawUsdRequests(r.UserId, "Withdraw");
                bd.DepositUsd = _dbFunc.GetDepositWithdrawUsdRequests(r.UserId, "Deposit");
                bd.Stats = _dbFunc.GetAlertStats(r.UserId);
                switch (uRole)
                {
                    case "Client":
                        bd.QrCode = au[0].ClientQrCode;
                        bd.Tether = au[0].ClientTetherAddress;
                        break;
                    case "Partner":
                        bd.QrCode = au[0].PartnerQrCode;
                        bd.Tether = au[0].PartnerTetherAddress;
                        break;
                    case "Investor":
                        bd.QrCode = au[0].InvestorQrCode;
                        bd.Tether = au[0].InvestorTetherAddress;
                        break;
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
                await _context.SaveChangesAsync();
            }
            catch (Exception)
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
    }
}