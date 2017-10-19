[![Build status](https://ci.appveyor.com/api/projects/status/github/dominicshaw/ssrs-image-creation?branch=master&amp;svg=true)](https://ci.appveyor.com/project/dominicshaw/ssrs-image-creation/branch/master)

# SSRS-Image-Creation
Very quick demo of taking a simple class and representing as an image

This very simple console app takes a class defined:

    class Program
    {
        static void Main(string[] args)
        {
            var rvm = new ReportListViewModel();

            ReportFactory.RunReport(rvm);

            Console.ReadKey();
        }
    }

    public class ReportListViewModel : List<ReportViewModel>
    {
        public ReportListViewModel()
        {
            Add(new ReportViewModel() { Name = "User 1", Score = 1 });
            Add(new ReportViewModel() { Name = "User 2", Score = 2 });
        }
    }

    public class ReportViewModel
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
    
.. and turns it into an image

![ScreenShot](https://raw.github.com/dominicshaw/SSRS-Image-Creation/master/ConsoleApplication5/Test%20Report%20Image.png)
