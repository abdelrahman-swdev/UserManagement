using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.ViewModels;

namespace UserManagement.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: Users
        public async Task<ActionResult> Index()
        {
            var users = await _userManager.Users
                .Select(user => new UserVM { 
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = _userManager.GetRolesAsync(user).Result
                })
                .ToListAsync();
            return View(users);
        }

        // GET: Users/Add
        public async Task<ActionResult> Save(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                // add new user
                return View("UserForm", new UserFormVM());
            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null) return NotFound("user not found");
                var model = new UserFormVM()
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    Id = user.Id,
                    UserName = user.UserName,
                };
                return View("UserForm", model);
            }
        }

        // GET: Users/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UserFormVM model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Id))
                {
                    // add new user
                    var user = new ApplicationUser()
                    {
                        FullName = model.FullName,
                        UserName = model.UserName,
                        Email = model.Email
                    };
                    IdentityResult result = new IdentityResult();

                    if(string.IsNullOrEmpty(model.Password)) result = await _userManager.CreateAsync(user);
                    if(!string.IsNullOrEmpty(model.Password)) result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        ViewBag.UserSaved = true;
                        return View("UserForm", model);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error, user didn't saved");
                        return View("UserForm", model);
                    }
                }
                else
                {
                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (user is null) return NotFound("User not found");
                    if (!string.IsNullOrEmpty(user.PasswordHash))
                    {
                        if (string.IsNullOrEmpty(model.Password)) {
                            ModelState.AddModelError("", "this user has password, enter the password to save changes");
                            return View("UserForm", model);
                        } 
                        if(!await _userManager.CheckPasswordAsync(user, model.Password))
                        {
                            ModelState.AddModelError("", "Password is incorrect");
                            return View("UserForm", model);
                        }
                        user.FullName = model.FullName;
                        user.Email = model.Email;
                        user.UserName = model.UserName;

                        IdentityResult updated = await _userManager.UpdateAsync(user);
                        if (updated.Succeeded)
                        {
                            ViewBag.UserSaved = true;
                            return View("UserForm", model);
                        }
                        ModelState.AddModelError("", "Error, user didn't saved");
                        return View("UserForm", model);
                    }

                    if(!string.IsNullOrEmpty(model.Password))
                    {
                        IdentityResult res = await _userManager.AddPasswordAsync(user, model.Password);
                        if (!res.Succeeded)
                        {
                            return View("UserForm", model);
                        }
                    }
                    user.FullName = model.FullName;
                    user.Email = model.Email;
                    user.UserName = model.UserName;

                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        ViewBag.UserSaved = true;
                        return View("UserForm", model);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error, user didn't saved");
                        return View("UserForm", model);
                    }
                }
            }
            else
            {
                return View("UserForm", model);
            }
        }

        public async Task<IActionResult> Delete([FromQuery]string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound("user not found");
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return BadRequest();
            }
        }
        // GET: Users/ManageRoles
        public async Task<ActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound("User not found");

            var roles = await _roleManager.Roles.ToListAsync();
            var model = new UserRolesVM()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(r => new RoleVM { 
                    RoleId = r.Id,
                    RoleName = r.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, r.Name).Result
                }).ToList()
            };

            return View(model);
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageRoles(UserRolesVM userRolesModel)
        {
            var user = await _userManager.FindByIdAsync(userRolesModel.UserId);
            if (user is null) return NotFound("User not found");

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach(var role in userRolesModel.Roles)
            {
                if(role.IsSelected && !userRoles.Any(r => r == role.RoleName))
                {
                    await _userManager.AddToRoleAsync(user, role.RoleName);
                }
                if (!role.IsSelected && userRoles.Any(r => r == role.RoleName))
                { 
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}