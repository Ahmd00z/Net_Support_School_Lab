using System;
using System.Drawing;
using System.Windows.Forms;
using ClassRoom.Learner.Services;
using ClassRoom.Shared.Models;

namespace ClassRoom.Learner.Forms
{
    public partial class MainForm : Form
    {
        private readonly LearnerService _service;
        private readonly string _learnerName;
        private readonly string _learnerId;
        private LockOverlayForm? _lockForm;
        private AssessmentForm? _assessmentForm;

        public MainForm(LearnerService service, string learnerName, string learnerId)
        {
            _service = service;
            _learnerName = learnerName;
            _learnerId = learnerId;
            InitializeComponent();
            SetupEvents();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void SetupEvents()
        {
            _service.LockReceived += (s, e) =>
            {
                Invoke(() => ShowLockScreen());
            };

            _service.UnlockReceived += (s, e) =>
            {
                Invoke(() => HideLockScreen());
            };

            _service.AssessmentReceived += (s, assessment) =>
            {
                Invoke(() => ShowAssessment(assessment));
            };

            _service.AssessmentEnded += (s, e) =>
            {
                Invoke(() => CloseAssessment());
            };
        }

        private void InitializeComponent()
        {
            this.Text = $"طالب - {_learnerName}";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Font = new Font("Cairo", 10);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(16, 185, 129)
            };

            var lblTitle = new Label
            {
                Text = $"مرحباً، {_learnerName}",
                Font = new Font("Cairo", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            headerPanel.Controls.Add(lblTitle);

            var lblId = new Label
            {
                Text = $"الكود: {_learnerId}",
                Font = new Font("Cairo", 9),
                ForeColor = Color.FromArgb(220, 252, 231),
                AutoSize = true,
                Location = new Point(20, 45)
            };
            headerPanel.Controls.Add(lblId);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            lblStatus = new Label
            {
                Text = "في انتظار المدرس...",
                Font = new Font("Cairo", 14),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            contentPanel.Controls.Add(lblStatus);

            var lblInfo = new Label
            {
                Text = "ستظهر الاختبارات هنا تلقائياً عندما يبدأها المدرس",
                Font = new Font("Cairo", 11),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(20, 60)
            };
            contentPanel.Controls.Add(lblInfo);

            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);
        }

        private Label lblStatus = null!;

        private void ShowLockScreen()
        {
            if (_lockForm == null || _lockForm.IsDisposed)
            {
                _lockForm = new LockOverlayForm();
            }
            _lockForm.Show();
            lblStatus.Text = "الشاشة مقفلة";
            lblStatus.ForeColor = Color.FromArgb(244, 63, 94);
        }

        private void HideLockScreen()
        {
            _lockForm?.Close();
            _lockForm = null;
            lblStatus.Text = "في انتظار المدرس...";
            lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
        }

        private void ShowAssessment(Assessment assessment)
        {
            if (_assessmentForm == null || _assessmentForm.IsDisposed)
            {
                _assessmentForm = new AssessmentForm(_service, assessment, _learnerName);
                _assessmentForm.FormClosed += (s, e) =>
                {
                    lblStatus.Text = "تم إنهاء الاختبار";
                    lblStatus.ForeColor = Color.FromArgb(16, 185, 129);
                };
            }
            _assessmentForm.Show();
            lblStatus.Text = "جاري الاختبار...";
            lblStatus.ForeColor = Color.FromArgb(139, 92, 246);
        }

        private void CloseAssessment()
        {
            _assessmentForm?.Close();
            _assessmentForm = null;
            lblStatus.Text = "تم إنهاء الاختبار";
            lblStatus.ForeColor = Color.FromArgb(16, 185, 129);
        }
    }
}
