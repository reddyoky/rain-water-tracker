using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WaterTracker.Data;
using WaterTracker.Models;

namespace WaterTracker.Controllers;

[Authorize]
[Route("api/water")]
[ApiController]
public class WaterApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public WaterApiController(ApplicationDbContext db) => _db = db;

    /// <summary>
    /// POST /api/water/drink
    /// Fetch API ile çağrılır. Sayfa yenilenmez.
    /// Bardağa tıklanınca bu endpoint tetiklenir.
    /// </summary>
    [HttpPost("drink")]
    public async Task<IActionResult> Drink([FromBody] DrinkRequest request)
    {
        // Cookie'den giriş yapan kullanıcının Id'sini al
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Yeni su kaydı oluştur
        var log = new WaterLog
        {
            UserId   = userId,
            AmountMl = request.AmountMl,
            DrankAt  = DateTime.Now
        };

        _db.WaterLogs.Add(log);
        await _db.SaveChangesAsync();

        // Güncel durumu JSON olarak döndür (JS bu veriyle ekranı günceller)
        return Ok(await GetDashboardData());
    }

    /// <summary>
    /// GET /api/water/status
    /// Sayfa ilk yüklendiğinde ve gerektiğinde çağrılır.
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> Status() => Ok(await GetDashboardData());

    private async Task<object> GetDashboardData()
    {
        var today = DateTime.Today;

        var users = await _db.Users
            .Include(u => u.WaterLogs.Where(l => l.DrankAt.Date == today))
            .Select(u => new
            {
                u.Id,
                u.DisplayName,
                u.DailyGoalMl,
                TotalMl    = u.WaterLogs.Sum(l => l.AmountMl),
                Percentage = u.DailyGoalMl > 0
                    ? (int)Math.Min(100, u.WaterLogs.Sum(l => l.AmountMl) * 100.0 / u.DailyGoalMl)
                    : 0
            })
            .ToListAsync();

        var recentLogs = await _db.WaterLogs
            .Include(l => l.AppUser)
            .Where(l => l.DrankAt.Date == today)
            .OrderByDescending(l => l.DrankAt)
            .Take(10)
            .Select(l => new
            {
                l.AppUser.DisplayName,
                l.AmountMl,
                DrankAt = l.DrankAt.ToString("HH:mm")
            })
            .ToListAsync();

        return new { users, recentLogs };
    }
}

// Fetch API'den gelen JSON body'yi karşılar
public record DrinkRequest(int AmountMl);
