using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bint.Models;
using System.Collections.Generic;
using Bint.Data;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Bint.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private static TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ILogger<ClientController> _logger;
        DBFunc dbf;
      
        public ClientController(ILogger<ClientController> logger, ApplicationDbContext context, UserManager<ApplicationUser> usermanager)
        {
            _logger = logger;
            _userManager = usermanager;
            _context = context;
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
                ClientDashboard cdb = new ClientDashboard();
                Payback pdb = new Payback();
                pdb.USDPaybackUser = dbf.GetUSDPaybackUser(_userManager.GetUserAsync(User).Result.UserId);
                cdb._payback = pdb;
                return View(cdb);
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
        public IActionResult Usd()
        {
            try
            {
                USDDashboard bd = new USDDashboard();
                var r = _userManager.GetUserAsync(User).Result;
                bd._requestUSD = dbf.GetRequestUSDReport(r.UserId);
                var urole = ControllerContext.ActionDescriptor.ControllerName;
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                bd._withdrawUSD = dbf.GetDepositWithdrawUSDRequests(r.UserId,"Withdraw");
                bd._depositUSD = dbf.GetDepositWithdrawUSDRequests(r.UserId,"Deposit");
                bd._Stats = dbf.GetAlertStats(r.UserId);
                if (urole=="Client")
                {
                    bd._qrcode = au[0].ClientQRCode;
                    bd._tether = au[0].ClientTetherAddress;
                }
                else if(urole=="Partner")
                {
                    bd._qrcode = au[0].PartnerQRCode;
                    bd._tether = au[0].PartnerTetherAddress;
                }
                else if(urole=="Investor")
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
        [Route("/client/TetherUpdate")]
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