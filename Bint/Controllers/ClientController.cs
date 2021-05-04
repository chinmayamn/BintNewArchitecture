using System;
using System.IO;
using System.Threading.Tasks;
using Bint.Constants;
using Bint.Data;
using Bint.Models;
using Bint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bint.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly ILogger<ClientController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbFunc _dbf;
        private readonly IDbConstants _dbConstants;
        private readonly IFileHelper _fileHelper;

        public ClientController(ILogger<ClientController> logger, ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, IConfiguration configuration, IDbConstants dbConstants, IFileHelper fileHelper)
        {
            _logger = logger;
            _userManager = userManager;
            _dbConstants = dbConstants;
            _fileHelper = fileHelper;
            _dbf = new DbFunc(_logger,configuration,dbConstants);
        }

        public IActionResult Dashboard()
        {
            try
            {
                var cdb = new ClientDashboard();
                var pdb = new Payback
                {
                    UsdPaybackUser = _dbf.GetUsdPaybackUser(_userManager.GetUserAsync(User).Result.UserId)
                };
                cdb.Payback = pdb;
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
                var ud = new UserProfileDoc();
                var id = _userManager.GetUserId(User);
                ud.UserDocs = _dbf.GetKycDocs(id);
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
                var bd = new UsdDashboard();
                var r = _userManager.GetUserAsync(User).Result;
                bd.RequestUsd = _dbf.GetRequestUsdReport(r.UserId);
                var uRole = ControllerContext.ActionDescriptor.ControllerName;
                var au = _userManager.GetUsersInRoleAsync("Admin").Result;
                bd.WithdrawUsd = _dbf.GetDepositWithdrawUsdRequests(r.UserId, "Withdraw");
                bd.DepositUsd = _dbf.GetDepositWithdrawUsdRequests(r.UserId, "Deposit");
                bd.Stats = _dbf.GetAlertStats(r.UserId);
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

        public IActionResult ActivityLog()
        {
            try
            {
                var r = _userManager.GetUserAsync(User).Result;
                var act = new ActivityLogDashboard
                {
                    ActivityLogTable = _dbf.GetUserActivityLog(r.UserId)
                };

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
        public async Task<IActionResult> TetherUpdate(string txtTether, [FromForm] IFormFile formFile)
        {
            try
            {
                var route = Request.Path.Value.Split("/")[1];
                var path = _fileHelper.DocumentUploadPath(formFile, "Tether");
                var u = _userManager.GetUserAsync(User).Result;


                //hard delete previous file
                try
                {
                    var t = Directory.GetCurrentDirectory() + "\\wwwroot" + u.QrCode.Replace("/", "\\");
                    var fileInfo = _fileHelper.GetFileInfo(t);
                    if (_fileHelper.Exists(fileInfo))
                        _fileHelper.Delete(fileInfo);
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