using System.Collections.Generic;
using System.Linq;

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
        public int Score { get { return Items.Sum(x => x.SubScore); } }

        public List<SubReportViewModel> Items { get; } = new List<SubReportViewModel>();
    }
}