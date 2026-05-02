using System;
using System.Drawing;
using System.Windows.Forms;
using ClassRoom.Learner.Services;

namespace ClassRoom.Learner.Forms
{
    public partial class ConnectForm : Form
    {
        private readonly LearnerService _service;
        private MainForm? _mainForm;

        public ConnectForm()
        {
            _service = new LearnerService();
            InitializeComponent();
            SetupEvents();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void SetupEvents()
        {
            _service.ConnectionStateChanged += (s, connected) =>
            {
                Invoke(() =>
                {
                    if (connected)
                    {
                        lblStatus.Text = "متصل بنجاح!";
                        lblStatus.ForeColor = Color.FromArgb(16, 185, 129);
                        btnConnect.Enabled = false;
                        btnConnect.Text = "متصل";
                    }
                    else
                    {
                        lblStatus.Text = "غير متصل";
                        lblStatus.ForeColor = Color.FromArgb(244, 63, 94);
                        btnConnect.Enabled = true;
                        btnConnect.Text = "اتصال";
                    }
                });
            };

            _service.IdConfirmed += (s, id) =>
            {
                Invoke(() =>
                {
                    _mainForm = new MainForm(_service, txtLearnerName.Text, id);
                    _mainForm.Show();
                    this.Hide();
                });
            };
        }

        private void InitializeComponent()
        {
            this.Text = "اتصال بالفصل - ClassRoom Control";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Font = new Font("Cairo", 10);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(14, 165, 233)
            };

            var lblTitle = new Label
            {
                Text = "اتصال بالفصل الدراسي",
                Font = new Font("Cairo", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            headerPanel.Controls.Add(lblTitle);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            var lblName = new Label
            {
                Text = "اسم الطالب:",
                Font = new Font("Cairo", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(30, 30)
            };
            contentPanel.Controls.Add(lblName);

            txtLearnerName = new TextBox
            {
                Width = 400,
                Height = 35,
                Location = new Point(30, 65),
                RightToLeft = RightToLeft.Yes,
                Font = new Font("Cairo", 12),
                BorderStyle = BorderStyle.FixedSingle
            };
            contentPanel.Controls.Add(txtLearnerName);

            var lblCode = new Label
            {
                Text = "كود الفصل:",
                Font = new Font("Cairo", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(30, 115)
            };
            contentPanel.Controls.Add(lblCode);

            txtClassroomCode = new TextBox
            {
                Width = 400,
                Height = 35,
                Location = new Point(30, 150),
                Text = "ROOM-101",
                RightToLeft = RightToLeft.Yes,
                Font = new Font("Cairo", 12),
                BorderStyle = BorderStyle.FixedSingle
            };
            contentPanel.Controls.Add(txtClassroomCode);

            btnConnect = new Button
            {
                Text = "اتصال",
                Width = 400,
                Height = 50,
                Location = new Point(30, 210),
                BackColor = Color.FromArgb(14, 165, 233),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 14, FontStyle.Bold)
            };
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.Click += BtnConnect_Click;
            contentPanel.Controls.Add(btnConnect);

            lblStatus = new Label
            {
                Text = "غير متصل",
                Font = new Font("Cairo", 11),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(30, 280),
                TextAlign = ContentAlignment.MiddleCenter
            };
            contentPanel.Controls.Add(lblStatus);

            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);
        }

        private TextBox txtLearnerName = null!;
        private TextBox txtClassroomCode = null!;
        private Button btnConnect = null!;
        private Label lblStatus = null!;

        private async void BtnConnect_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLearnerName.Text))
            {
                MessageBox.Show("الرجاء إدخال اسم الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await _service.ConnectAsync(txtLearnerName.Text, txtClassroomCode.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"فشل الاتصال: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            await _service.DisconnectAsync();
            base.OnFormClosing(e);
        }
    }
}
