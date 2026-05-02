using System;
using System.Windows.Forms;
using ClassRoom.ExamBuilder.Forms;

namespace ClassRoom.ExamBuilder
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
