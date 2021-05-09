using System;
using System.IO;
using System.Threading.Tasks;
using Bint.Models;
using Bint.Services;
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
        private readonly ILogger<InvestorApiController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileHelper _fileHelper;

        public InvestorApiController(UserManager<ApplicationUser> userManager,
            ILogger<InvestorApiController> logger, IFileHelper fileHelper)
        {
            _userManager = userManager;
            _fileHelper = fileHelper;
            _logger = logger;
        }

        [HttpPost]
        [Route("/investor/TetherUpdate")]
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
                    _fileHelper.HardDeleteFile(t);
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
        [HttpGet]
        public string Get()
        {
            return  "ss";
        }
    }
}