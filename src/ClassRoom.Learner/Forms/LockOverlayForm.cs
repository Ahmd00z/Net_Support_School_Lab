using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClassRoom.Learner.Forms
{
    public partial class LockOverlayForm : Form
    {
        public LockOverlayForm()
        {
            InitializeComponent();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.TopMost = true;
            this.ShowInTaskbar = false;

            var lblIcon = new Label
            {
                Text = "🔒",
                Font = new Font("Segoe UI Emoji", 72),
                ForeColor = Color.FromArgb(244, 63, 94),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblMessage = new Label
            {
                Text = "الشاشة مقفلة",
                Font = new Font("Cairo", 28, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblSubMessage = new Label
            {
                Text = "يرجى الانتظار حتى يفتح المدرس الشاشة",
                Font = new Font("Cairo", 14),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            this.Controls.Add(lblIcon);
            this.Controls.Add(lblMessage);
            this.Controls.Add(lblSubMessage);

            this.Load += (s, e) =>
            {
                int centerX = this.ClientSize.Width / 2;
                int centerY = this.ClientSize.Height / 2;

                lblIcon.Location = new Point(centerX - lblIcon.Width / 2, centerY - 120);
                lblMessage.Location = new Point(centerX - lblMessage.Width / 2, centerY);
                lblSubMessage.Location = new Point(centerX - lblSubMessage.Width / 2, centerY + 60);
            };
        }
    }
}
