using System;
using System.Windows.Forms;
using ClassRoom.Instructor.Forms;

namespace ClassRoom.Instructor
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new DashboardForm());
        }
    }
}
