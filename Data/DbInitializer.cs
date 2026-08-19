using WaterTracker.Data;
using WaterTracker.Models;

namespace WaterTracker.Data;

/// <summary>
/// Uygulama ilk açıldığında çalışır.
/// Veritabanı boşsa, Taki ve Yağmur'u oluşturur.
/// </summary>
public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Zaten kullanıcı varsa çıkıyoruz
        if (context.Users.Any()) return;

        var users = new[]
        {
            new AppUser
            {
                Username    = "taki",
                Password    = BCrypt.Net.BCrypt.HashPassword("sifre123"),
                DisplayName = "Taki 💙",
                DailyGoalMl = 2500
            },
            new AppUser
            {
                Username    = "yagmur",
                Password    = BCrypt.Net.BCrypt.HashPassword("sifre123"),
                DisplayName = "Yağmur 🌧️",
                DailyGoalMl = 2000
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }
}
