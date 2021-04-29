using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bint.Repository;
using Bint.Models;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Bint.Controllers
{
    [Produces("application/json")]
    [Route("api/Investor")]
    public class InvestorApiController : Controller
    {
        private IInvestorRepository _investorRepository;
        private UserManager<ApplicationUser> _userManager;
        private readonly ILogger<InvestorApiController> _logger;
        public InvestorApiController(IInvestorRepository investorRepository, UserManager<ApplicationUser> usermanager, ILogger<InvestorApiController> logger)
        {
            _investorRepository = investorRepository;
            _userManager = usermanager;
            _logger = logger;
        }

        [HttpPost]
        [Route("/investor/TetherUpdate")]
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