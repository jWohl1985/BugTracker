namespace BugTracker.Models.ChartModels
{
    public class AmChartData
    {
        public AmItem[] Data { get; set; } = default!;
    }

    public class AmItem
    {
        public string Project { get; set; } = default!;
        public int Tickets { get; set; }
        public int Developers { get; set; }
    }
}
