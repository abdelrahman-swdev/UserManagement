using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.ViewModels;

namespace UserManagement.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        // GET: RolesController
        public async Task<ActionResult> Index()
        {
            return View(await _roleManager.Roles.ToListAsync());
        }

        // POST: RolesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddRoleVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await _roleManager.Roles.ToListAsync());
            }
            
            if (await _roleManager.RoleExistsAsync(model.RoleName))
            {
                ModelState.AddModelError("", "Role is exists");
                return View("Index", await _roleManager.Roles.ToListAsync());
            }
            await _roleManager.CreateAsync(new IdentityRole() { Name = model.RoleName.Trim() });
            return RedirectToAction(nameof(Index));
            
        }

        public async Task<ActionResult> Delete(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if(role is not null)
            {
                await _roleManager.DeleteAsync(role);
                return RedirectToAction(nameof(Index));
            }
            return NotFound("Role not found");
        }
    }
}
