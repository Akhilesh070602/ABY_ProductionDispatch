namespace PackingDisplay.Models
{
    public class DashboardDto
    {
        public decimal TodayProduction { get; set; }
        public decimal MonthProduction { get; set; }
        public int WorkingDays { get; set; }
        public decimal Target { get; set; }

        // ✅ Running Average = MonthProduction / WorkingDays
        public decimal RunningAvg => WorkingDays == 0 ? 0 : Math.Round(MonthProduction / WorkingDays, 2);


        // ✅ Required Average = (Target - MonthProduction) / Remaining Working Days from Today
        public decimal RequiredAvg
        {
            get
            {
                int daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                int currentDay = DateTime.Today.Day;

                // Remaining days from today (not including today)
                int remainingDays = daysInMonth - currentDay;

                // Safety check to avoid divide by zero
                if (remainingDays <= 0)
                    remainingDays = 1;

                // Required average = remaining target / remaining days
                return Math.Round((Target - MonthProduction) / remainingDays, 2);
            }
        }

        // ✅ Today % vs Required Avg
        public decimal TodayPercentage => RequiredAvg == 0 ? 0 : Math.Round((TodayProduction / RequiredAvg) * 100, 2);

        // ✅ Month % vs Target
        public decimal MonthPercentage => Target == 0 ? 0 : Math.Round((MonthProduction / Target) * 100, 2);
    }
}