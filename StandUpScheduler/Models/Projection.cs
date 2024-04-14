namespace StandUpScheduler.Models;

public class Projection
{
    public required string Color { get; set; }
    public required string Description { get; set; }
    public DateTimeOffset Start { get; set; }
    public int Minutes { get; set; }
    public DateTimeOffset End => Start.AddMinutes(Minutes);
}