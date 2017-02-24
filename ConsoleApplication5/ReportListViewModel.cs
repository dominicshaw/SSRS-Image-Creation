using System.Collections.Generic;

namespace ConsoleApplication5
{
    public class ReportListViewModel : List<ReportViewModel>
    {
        public ReportListViewModel()
        {
            Add(new ReportViewModel() { Name = "User 1", Score = 1 });
            Add(new ReportViewModel() { Name = "User 2", Score = 2 });
        }
    }
}