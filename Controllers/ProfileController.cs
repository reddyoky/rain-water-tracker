using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WaterTracker.Data;

namespace WaterTracker.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ProfileController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    // GET /Profile
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user   = await _db.Users.FindAsync(userId);
        if (user == null) return RedirectToAction("Login", "Auth");
        return View(user);
    }

    // POST /Profile/UploadPhoto
    [HttpPost]
    public async Task<IActionResult> UploadPhoto(IFormFile photo)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user   = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (photo == null || photo.Length == 0)
        {
            TempData["PhotoError"] = "Lütfen bir dosya seç.";
            return RedirectToAction(nameof(Index));
        }

        // Boyut kontrolü: max 5MB
        if (photo.Length > 5 * 1024 * 1024)
        {
            TempData["PhotoError"] = "Maksimum dosya boyutu 5MB olabilir.";
            return RedirectToAction(nameof(Index));
        }

        // Uzantı kontrolü
        var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        if (!allowed.Contains(ext))
        {
            TempData["PhotoError"] = "Sadece JPG, PNG, WEBP veya GIF yükleyebilirsin.";
            return RedirectToAction(nameof(Index));
        }

        // Uploads klasörü
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);

        // Eski fotoğrafı sil
        if (!string.IsNullOrEmpty(user.ProfilePhotoPath))
        {
            var oldPath = Path.Combine(_env.WebRootPath, user.ProfilePhotoPath.TrimStart('/'));
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
        }

        // Yeni dosya adı: userId_timestamp.ext
        var fileName  = $"{userId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{ext}";
        var filePath  = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await photo.CopyToAsync(stream);

        user.ProfilePhotoPath = $"/uploads/{fileName}";
        await _db.SaveChangesAsync();

        TempData["PhotoSuccess"] = "Profil fotoğrafın güncellendi! 🎉";
        return RedirectToAction(nameof(Index));
    }

    // POST /Profile/ChangePassword
    [HttpPost]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user   = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        // Mevcut şifre doğrulama
        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
        {
            TempData["PwError"] = "Mevcut şifren yanlış.";
            return RedirectToAction(nameof(Index));
        }

        // Yeni şifre eşleşme kontrolü
        if (newPassword != confirmPassword)
        {
            TempData["PwError"] = "Yeni şifreler eşleşmiyor.";
            return RedirectToAction(nameof(Index));
        }

        if (newPassword.Length < 6)
        {
            TempData["PwError"] = "Şifre en az 6 karakter olmalı.";
            return RedirectToAction(nameof(Index));
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _db.SaveChangesAsync();

        TempData["PwSuccess"] = "Şifren başarıyla değiştirildi! 🔒";
        return RedirectToAction(nameof(Index));
    }
}
