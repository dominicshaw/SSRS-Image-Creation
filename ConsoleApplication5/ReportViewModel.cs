using System.Collections.Generic;

namespace ConsoleApplication5
{
    public class SubReportViewModel
    {
        public int UserID { get; set; }
        public string SubName { get; set; }
        public int SubScore { get; set; }
    }

    public class ReportViewModel
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public List<SubReportViewModel> Items { get; set; } = new List<SubReportViewModel>();
    }
}