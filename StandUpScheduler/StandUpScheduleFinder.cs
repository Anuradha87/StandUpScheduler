using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StandUpScheduler.Models;
using StandUpScheduler.Models.ViewModel;

namespace StandUpScheduler;

public class StandUpScheduleFinder(HttpClient httpClient, IConfiguration configuration) : IStandUpScheduleFinder
{
    private static readonly DateTime DateOfRun = DateTime.Parse("2015-12-14"); // DateTime.Now;

    public async Task FindStandUpSchedules()
    {
        var scheduleUrl = $"{configuration["ScheduleUrl"]}{DateOfRun:yyyy-MM-dd}.json";
        var timeSchedule = await GetJsonFromUrl<TeamSchedule>(scheduleUrl);
        var schedules = timeSchedule?.ScheduleResult.Schedules;

        if (schedules is null)
        {
            Console.WriteLine("No schedules found");
            return;
        }

        var (workStart, workEnd) = CalculateWorkTimes(schedules);

        while (true)
        {
            Console.WriteLine("\nEnter the required number of members for the meeting (between 2 and 16):");
            var input = Console.ReadLine();

            if (int.TryParse(input, out var requiredMembers))
            {
                if (requiredMembers is >= 2 and <= 16)
                {
                    var optimalTimes = FindOptimalMeetingTimes(schedules, requiredMembers, workStart, workEnd);

                    foreach (var slot in optimalTimes)
                    {
                        var joinedPersonIds = string.Join(", ", slot.Employees.Select(e => e.Name));
                        Console.WriteLine(
                            $"Time Slot: {slot.Start:HH:mm} - {slot.End:HH:mm}, Number of Members: {slot.Employees.Count} ," +
                            $" Members: {joinedPersonIds}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Number of members must be between 2 and 16.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
    }

    private static (DateTimeOffset workStart, DateTimeOffset workEnd) CalculateWorkTimes(
        IEnumerable<Schedule> schedules)
    {
        var defaultStart = new DateTimeOffset(new DateTime(DateOfRun.Year, DateOfRun.Month, DateOfRun.Day, 00, 00, 00),
            TimeSpan.Zero);
        var defaultEnd = new DateTimeOffset(new DateTime(DateOfRun.Year, DateOfRun.Month, DateOfRun.Day, 23, 59, 59),
            TimeSpan.Zero);

        var earliestStart = schedules?
            .Min(s => s.OverallStart) ?? defaultStart;

        var latestEnd = schedules
            .Max(s => s.OverallEnd) ?? defaultEnd;

        var workStart = earliestStart < defaultStart ? earliestStart : defaultStart;
        var workEnd = latestEnd > defaultEnd ? latestEnd : defaultEnd;

        return (workStart, workEnd);
    }

    public static List<StandUpSchedulesViewModel> FindOptimalMeetingTimes(IEnumerable<Schedule> schedules,
        int requiredMembers,
        DateTimeOffset workStart,
        DateTimeOffset workEnd)
    {
        var interval = TimeSpan.FromMinutes(15);
        var scheduleSlots = new List<StandUpSchedulesViewModel>();

        for (var time = workStart; time <= workEnd; time = time.AddMinutes(interval.TotalMinutes))
        {
            var slotStart = time;
            var slotEnd = time.AddMinutes(interval.TotalMinutes);

            var availableEmployees = schedules
                .Where(s => !s.IsFullDayAbsence && s.ContractTimeMinutes > 0 && slotStart >= s.OverallStart &&
                            slotEnd <= s.OverallEnd)
                .Where(s => s.Projection.All(p =>
                    !((slotStart < p.End && slotEnd > p.Start) &&
                      (p.Description == "Lunch" || p.Description == "Short break"))
                ))
                .Select(s => new Employee { Id = s.PersonId, Name = s.Name }).ToList();

            if (availableEmployees.Count > 0 && requiredMembers <= availableEmployees.Count)
            {
                scheduleSlots.Add(new StandUpSchedulesViewModel
                {
                    Start = slotStart,
                    End = slotEnd,
                    Employees = availableEmployees
                });
            }
        }

        return scheduleSlots;
    }

    private async Task<T?> GetJsonFromUrl<T>(string? url)
    {
        var response = await httpClient.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine("Error fetching Date From URL");
            return default;
        }

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(json);
    }
}