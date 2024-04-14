namespace StandUpScheduler.Models.ViewModel;

public class StandUpSchedulesViewModel
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    
    public List<Employee> Employees { get; set; } = new();
}

public class Employee
{
    public string  Id { get; set; }
    public string Name { get; set; }
}