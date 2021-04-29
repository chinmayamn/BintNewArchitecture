using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bint.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bint.Controllers
{
    public class RoleController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RoleController> _logger;
        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> usermanager, ILogger<RoleController> logger)
        {
            _roleManager = roleManager;_userManager = usermanager; _logger = logger;
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
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Required]string name)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    if (await _roleManager.RoleExistsAsync(name))
                    {
                        TempData["error"] = "Role already exists";
                        _logger.LogError("Role already exists {name}", name);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        string z = name.ToLower();
                        z = z.First().ToString().ToUpper() + z.Substring(1);

                        IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(z));
                        if (result.Succeeded)
                        {
                            TempData["data"] = "Role created";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Errors(result);
                            TempData["error"] = result;
                            _logger.LogError(result.ToString());
                        }
                    }
                }

                return View(name);
            }
        
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {

           
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var rc = _userManager.GetUsersInRoleAsync(role.Name).Result.Count;
                if (rc >0)
                {
                    TempData["error"] = "Users are present for this role. Cant delete";
                        _logger.LogError("Users are present for this role. Cant delete {id}", id);
                        return View("Index", _roleManager.Roles);
                }
                else
                {

                   IdentityResult result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        TempData["data"] = "No users are present in role. Role deleted";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = result;
                            _logger.LogError(result.ToString());
                            Errors(result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "No role found"); TempData["error"] = "No role found";

            }
            return View("Index", _roleManager.Roles);
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

         
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            var r = _roleManager.Roles;
            List<ApplicationUser> members = new List<ApplicationUser>();
            List<ApplicationUser> nonMembers = new List<ApplicationUser>();
                
            foreach (ApplicationUser user in _userManager.Users)
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
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            Errors(result);
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userId);
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
            else
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
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}