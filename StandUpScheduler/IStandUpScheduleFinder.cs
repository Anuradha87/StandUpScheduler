using StandUpScheduler.Models.ViewModel;

namespace StandUpScheduler;

public interface IStandUpScheduleFinder
{  
   public  Task FindStandUpSchedules();
}