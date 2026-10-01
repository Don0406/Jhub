using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _UPDATED__FrontEnd_Capstone.Models;

namespace _UPDATED__FrontEnd_Capstone.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("")] 
    [Route("Home/Login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewBag.ErrorMessage = "Fields cannot be empty.";
            return View();
        }

        string cleanEmail = email.ToLower().Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == cleanEmail && u.Password == password);

        if (user == null)
        {
            ViewBag.ErrorMessage = "Invalid credentials.";
            return View();
        }

        // 1. Suriin kung Pending ang account
        if (user.Status == "Pending")
        {
            ViewBag.ErrorMessage = "Wait for admin approval. Try logging in later.";
            return View();
        }

        // 2. Suriin kung Rejected ang account
        if (user.Status == "Rejected")
        {
            ViewBag.ErrorMessage = "Your account has been rejected by the admin.";
            return View();
        }
        
        // 3. Kung Active na (o walang status na nakalagay), ituloy ang pagpasok ayon sa role
        string userRole = user.Role?.ToLower() ?? "operator";

        return userRole switch
        {
            "admin"      => RedirectToAction("UserManagement", "User"),
            "operator"   => RedirectToAction("OperatorDashboard", "Tour"),
            _            => RedirectToAction("Login")
        };
    }

    [HttpGet]
    [Route("Home/Register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [Route("Home/Register")]
    public async Task<IActionResult> Register(string fullName, string email, string phoneNumber, string sex, int age, string password, string confirmPassword, string role)
    {
        if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || 
            string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(sex) || 
            age <= 0 || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
        {
            ViewBag.ErrorMessage = "All fields are required.";
            return View();
        }

        if (password != confirmPassword)
        {
            ViewBag.ErrorMessage = "Passwords do not match.";
            return View();
        }
     
        string cleanEmail = email.ToLower().Trim();

        bool emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == cleanEmail);
        if (emailExists)
        {
            ViewBag.ErrorMessage = "Email is already registered.";
            return View();
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
            Status = "Pending" // Naka-set sa Pending para hintayin muna ang approval ng admin
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        // I-pasa ang mensahe sa TempData patungo sa Login page pagkatapos mag-register
        TempData["SuccessMessage"] = "Wait for admin approval. Try logging in later.";

        return RedirectToAction("Login", "Home");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    [Route("Home/Logout")]
    public IActionResult Logout()
    {
        return RedirectToAction("Login");
    }
}
