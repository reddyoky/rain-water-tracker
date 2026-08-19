using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WaterTracker.Data;

namespace WaterTracker.Controllers;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _db;

    public AuthController(ApplicationDbContext db) => _db = db;

    // GET /Auth/Login
    [HttpGet]
    public IActionResult Login()
    {
        // Zaten giriş yapmışsa dashboard'a gönder
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    // POST /Auth/Login
    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // 1. Kullanıcıyı veritabanında bul
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        // 2. Kullanıcı yoksa veya şifre yanlışsa hata göster
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            ViewBag.Error = "❌ Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        // 3. Claim'leri oluştur (cookie'ye kaydedilecek bilgiler)
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name,           user.Username),
            new("DisplayName",             user.DisplayName)
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // 4. Cookie'yi yaz
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }

    // GET /Auth/Logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
