using EmirsCorrespondence.Data;
using EmirsCorrespondence.Data.DTO;
using EmirsCorrespondence.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace EmirsCorrespondence.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        //private readonly IPasswordHasher<IdentityUser> _passwordHasher;

        //public UserController(ApplicationDbContext context, IPasswordHasher<IdentityUser> passwordHasher)
        //{
        //    _context = context;
        //    //_passwordHasher = passwordHasher;
        //}

        // GET: List Users
        public IActionResult Index()
        {
            var users = _context.Users
                .Include(u => u.Role)
                .Select(u => new UserViewModel
                {
                    UserId = u.UserId,
                    Username = u.UserName,
                    Email = u.Email,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive
                })
                .ToList();

            return View(users);
        }

        // GET: Create User Form
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Create User
        [HttpPost]
        public IActionResult Create(Users user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Edit User
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // POST: Update User
        [HttpPost]
        public IActionResult Edit(Users user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(new Users { UserId = user.UserId, UserName = user.UserName, Email = user.Email, RoleId = user.RoleId, IsActive = user.IsActive });
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // POST: Delete User
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Activate/Deactivate User
        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
