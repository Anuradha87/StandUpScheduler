namespace StandUpScheduler.Models;

public class Schedule
{
    public int ContractTimeMinutes { get; set; }
    
    public DateTimeOffset Date { get; set; }
    public bool IsFullDayAbsence { get; set; }
    public required string Name { get; set; }
    public required string PersonId { get; set; }
    public List<Projection> Projection { get; set; } = new();
    public DateTimeOffset? OverallStart => Projection.Count > 0 ? Projection.Min(p => p.Start) : null;
    public DateTimeOffset? OverallEnd => ContractTimeMinutes != 0 ? OverallStart?.AddMinutes(ContractTimeMinutes +60) : null;
}