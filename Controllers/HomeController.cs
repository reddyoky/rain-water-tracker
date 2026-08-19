using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterTracker.Data;

namespace WaterTracker.Controllers;

[Authorize] // ← Giriş yapmadan bu sayfaya gelinemez!
public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        // Tüm kullanıcıları, bugünkü su logları ile birlikte getir
        var users = await _db.Users
            .Include(u => u.WaterLogs.Where(l => l.DrankAt.Date == today))
            .ToListAsync();

        // Bardak türleri (butonlar için)
        var glassTypes = await _db.GlassTypes.ToListAsync();

        // Timeline: Bugünkü son 20 kayıt (en yeni en üstte)
        var timeline = await _db.WaterLogs
            .Include(l => l.AppUser)
            .Where(l => l.DrankAt.Date == today)
            .OrderByDescending(l => l.DrankAt)
            .Take(20)
            .ToListAsync();

        ViewBag.Users      = users;
        ViewBag.GlassTypes = glassTypes;
        ViewBag.Timeline   = timeline;

        return View();
    }
}
