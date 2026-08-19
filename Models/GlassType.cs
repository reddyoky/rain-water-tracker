using System.ComponentModel.DataAnnotations;

namespace WaterTracker.Models;

public class GlassType
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public int CapacityMl { get; set; }

    [MaxLength(10)]
    public string Emoji { get; set; } = "💧";
}
