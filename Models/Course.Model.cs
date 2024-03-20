// 

namespace KIITStarter.Models;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsPaid { get; set; }
    public float? Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? Author { get; set; }

    // Convert local DateTime to UTC before saving
    public void ConvertToLocalTime()
    {
        if (CreatedAt.Kind != DateTimeKind.Utc)
        {
            CreatedAt = DateTime.SpecifyKind(CreatedAt, DateTimeKind.Utc);
        }
    }

}