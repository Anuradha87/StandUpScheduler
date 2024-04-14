using NUnit.Framework;
using StandUpScheduler.Models;


namespace StandUpScheduler.Tests
{
    [TestFixture]
    public class StandUpScheduleFinderTests
    {
        [Test]
        public void FindOptimalMeetingTimes_FindsCorrectSlots()
        {
            var schedules = new List<Schedule>
            {
                new Schedule
                {
                    PersonId = "00488204-23f2-4458-8469-16ec67f2483d",
                    Name = "Alice",
                    IsFullDayAbsence = false,
                    ContractTimeMinutes = 480,
                  
                    Projection =
                    [
                        new Projection
                        {
                            Start = new DateTimeOffset(new DateTime(2023, 04, 14, 08, 00, 00), TimeSpan.Zero),
                            Description = "Social Media",
                            Color = "color",
                            Minutes = 240
                        },

                        new Projection
                        {
                            Start = new DateTimeOffset(new DateTime(2023, 4, 14, 12, 00, 00), TimeSpan.Zero),
                            Description = "Phone Call",
                            Color = "color",
                            Minutes = 60
                        },

                        new Projection
                        {
                            Start = new DateTimeOffset(new DateTime(2023, 4, 14, 13, 00, 00), TimeSpan.Zero),
                            Description = "Lunch",
                            Color = "color",
                            Minutes = 60
                        }
                    ]
                },
                new Schedule
                {
                    PersonId = "05db3441-60d8-487a-b702-1b3bcb860af4",
                    Name = "Bob",
                    IsFullDayAbsence = false,
                    ContractTimeMinutes = 480,
                    Projection =
                    [
                        new Projection
                        {
                            Start = new DateTimeOffset(new DateTime(2023, 4, 14, 08, 00, 00), TimeSpan.Zero),
                            Description = "Social Media",
                            Color = "color",
                            Minutes = 240
                        },

                        new Projection
                        {
                            Start = new DateTimeOffset(new DateTime(2023, 4, 14, 12, 00, 00), TimeSpan.Zero),
                            Description = "Lunch",
                            Color = "color",
                            Minutes = 60
                        },

                        new Projection
                        {
                            Start = new DateTimeOffset(new DateTime(2023, 4, 14, 13, 00, 00), TimeSpan.Zero),
                            Description = "Phone Call",
                            Color = "color",
                            Minutes = 60
                        }
                    ]
                }
            };
            var requiredMembers = 1;
            var workStart = new DateTimeOffset(new DateTime(2023, 4, 14, 8, 0, 0), TimeSpan.Zero);
            var workEnd = new DateTimeOffset(new DateTime(2023, 4, 14, 18, 0, 0), TimeSpan.Zero);

            // Act
            var result = StandUpScheduleFinder.FindOptimalMeetingTimes(schedules, requiredMembers, workStart, workEnd);

            // Assert
            Assert.AreEqual(36, result.Count); 
            var slot = result.First();
            Assert.AreEqual(new DateTimeOffset(new DateTime(2023, 4, 14, 8, 0, 0), TimeSpan.Zero), slot.Start);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2023, 4, 14, 8, 15, 0), TimeSpan.Zero), slot.End);
            Assert.AreEqual(2, slot.Employees.Count);
        }
    }
}