using System;

namespace ConsoleApplication5
{
    class Program
    {
        static void Main(string[] args)
        {
            var rvm = new ReportListViewModel();

            ReportFactory.RunReport(rvm);

            Console.ReadKey();
        }
    }
}
