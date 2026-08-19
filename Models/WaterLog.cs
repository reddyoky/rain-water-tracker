namespace WaterTracker.Models;

public class WaterLog
{
    public int Id { get; set; }

    // Foreign Key → AppUser
    public int UserId { get; set; }
    public AppUser AppUser { get; set; } = null!; // null! = "ben garantileyiyorum, null olmayacak"

    public int AmountMl { get; set; }

    public DateTime DrankAt { get; set; } = DateTime.Now; // Yerel saat
}
