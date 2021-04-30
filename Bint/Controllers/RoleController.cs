using Bint.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bint.Controllers
{
    public class RoleController : Controller
    {
        private readonly ILogger<RoleController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
            ILogger<RoleController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public ViewResult Index()
        {
            try
            {
                return View(_roleManager.Roles);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return View();
        }

        public async Task<IActionResult> Update(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                var members = new List<ApplicationUser>();
                var nonMembers = new List<ApplicationUser>();

                foreach (var user in _userManager.Users)
                {
                    var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;

                    list.Add(user);
                }

                return View(new RoleEdit
                {
                    Role = role,
                    Members = members,
                    NonMembers = nonMembers
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result;
                    foreach (var userId in model.AddIds ?? new string[] { })
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            result = await _userManager.AddToRoleAsync(user, model.RoleName);
                            if (!result.Succeeded)
                                Errors(result);
                        }
                    }

                    foreach (var userId in model.DeleteIds ?? new string[] { })
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                            if (!result.Succeeded)
                                Errors(result);
                        }
                    }
                }

                if (ModelState.IsValid)
                    return RedirectToAction(nameof(Index));
                return await Update(model.RoleId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return View();
        }

        private void Errors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}