using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _UPDATED__FrontEnd_Capstone.Models;
using System.Threading.Tasks;

namespace _UPDATED__FrontEnd_Capstone.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /User/UserManagement
        [HttpGet]
        public async Task<IActionResult> UserManagement()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // POST: /User/CreateUser (Admin-created users are Active/Approved immediately)
        [HttpPost]
        public async Task<IActionResult> CreateUser(string fullName, string email, string phoneNumber, string sex, int age, string password, string role)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(sex) || 
                age <= 0 || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                return RedirectToAction(nameof(UserManagement));
            }

            string cleanEmail = email.ToLower().Trim();

            bool emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == cleanEmail);
            if (emailExists)
            {
                return RedirectToAction(nameof(UserManagement));
            }

            var newUser = new User
            {
                FullName = fullName.Trim(),
                Email = cleanEmail,
                PhoneNumber = phoneNumber.Trim(),
                Sex = sex.ToLower().Trim(),
                Age = age,
                Password = password,
                Role = role.ToLower().Trim(),
                Status = "Active" // Active na agad kapag admin ang gumawa
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(UserManagement));
        }

        // POST: /User/EditUser
        [HttpPost]
        public async Task<IActionResult> EditUser(int id, string fullName, string email, string password, string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.FullName = fullName?.Trim();
                user.Email = email?.ToLower().Trim();
                user.Password = password;
                user.Role = role?.ToLower().Trim();

                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(UserManagement));
        }

        // POST: /User/ApproveUser
        [HttpPost]
        public async Task<IActionResult> ApproveUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Status = "Active";
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(UserManagement));
        }

        // POST: /User/RejectUser
        [HttpPost]
        public async Task<IActionResult> RejectUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Status = "Rejected";
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(UserManagement));
        }

        // POST: /User/DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(UserManagement));
        }

        [HttpGet]
        public IActionResult OperatorDashboard()
        {
            return View();
        }
    }
}
