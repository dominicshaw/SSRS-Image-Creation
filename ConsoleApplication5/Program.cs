using System;
using System.Reflection;
using log4net;

namespace ConsoleApplication5
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalContext.Properties["username"] = Environment.UserName;
            GlobalContext.Properties["version"] = Assembly.GetExecutingAssembly().GetName().Version;

            var rvm = new ReportListViewModel();

            ReportFactory.RunReport(rvm);

            Console.ReadKey();
        }
    }
}
