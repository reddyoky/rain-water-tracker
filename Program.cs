using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using WaterTracker.Data;

var builder = WebApplication.CreateBuilder(args);

// ── 1. MVC ──────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── 2. Veritabanı: Lokal → SQLite | Railway → PostgreSQL ──────────────────
//    Railway, DATABASE_URL environment variable'ını otomatik ayarlar.
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Production (Railway): PostgreSQL
    // Railway'in URL formatı: postgresql://user:pass@host:5432/dbname
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(databaseUrl));
}
else
{
    // Lokal geliştirme: SQLite (kurulum gerektirmez)
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connStr));
}

// ── 3. Cookie Authentication ─────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath         = "/Auth/Login";
        options.LogoutPath        = "/Auth/Logout";
        options.AccessDeniedPath  = "/Auth/Login";
        options.Cookie.Name       = "RainWaterTrackerAuth";
        options.ExpireTimeSpan    = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// ── Middleware Pipeline ───────────────────────────────────────────────────
var app = builder.Build();

// Veritabanını oluştur + seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    DbInitializer.Initialize(db);
}

// Profil fotoğrafı klasörünü garanti et
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
    Directory.CreateDirectory(uploadsPath);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
