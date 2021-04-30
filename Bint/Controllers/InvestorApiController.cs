using System;
using System.IO;
using System.Threading.Tasks;
using Bint.Models;
using Bint.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bint.Controllers
{
    [Produces("application/json")]
    [Route("api/Investor")]
    public class InvestorApiController : Controller
    {
        private readonly IInvestorRepository _investorRepository;
        private readonly ILogger<InvestorApiController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvestorApiController(IInvestorRepository investorRepository, UserManager<ApplicationUser> userManager,
            ILogger<InvestorApiController> logger)
        {
            _investorRepository = investorRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost]
        [Route("/investor/TetherUpdate")]
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