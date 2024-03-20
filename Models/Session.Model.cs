namespace KIITStarter.Models;


public class Session
{
    public int Id { get; set; }

    public int User { get; set; }

    public string token { get; set; }

    public DateTime createdAt { get; set; }
    public DateTime? lastActivityAt { get; set; }
}