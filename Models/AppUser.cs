using System.ComponentModel.DataAnnotations;

namespace WaterTracker.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty; // BCrypt hash

    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    public int DailyGoalMl { get; set; } = 2000;

    // Profil fotoğrafı yolu (örn: /uploads/1_photo.jpg)
    // null ise varsayılan avatar gösterilir
    public string? ProfilePhotoPath { get; set; }

    // Navigation property
    public ICollection<WaterLog> WaterLogs { get; set; } = new List<WaterLog>();
}
