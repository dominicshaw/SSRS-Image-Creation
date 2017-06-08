using System.Collections.Generic;

namespace ConsoleApplication5
{
    public class ReportListViewModel : List<ReportViewModel>
    {
        public ReportListViewModel()
        {
            Add(new ReportViewModel()
            {
                UserID = 1,
                Name = "User 1",
                Score = 1,
                Items =
                {
                    new SubReportViewModel() { UserID = 1, SubName = "Detail 1", SubScore = 10 },
                    new SubReportViewModel() { UserID = 1, SubName = "Detail 2", SubScore = 20 },
                }
            });
            Add(new ReportViewModel()
            {
                UserID = 2,
                Name = "User 2",
                Score = 2,
                Items =
                {
                    new SubReportViewModel() { UserID = 2, SubName = "Detail 3", SubScore = 30 },
                    new SubReportViewModel() { UserID = 2, SubName = "Detail 4", SubScore = 40 },
                }
            });
        }
    }
}