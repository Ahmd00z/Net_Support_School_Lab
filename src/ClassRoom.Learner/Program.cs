using System;
using System.Windows.Forms;
using ClassRoom.Learner.Forms;

namespace ClassRoom.Learner
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new ConnectForm());
        }
    }
}
