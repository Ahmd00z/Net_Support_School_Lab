using System;
using System.Drawing;
using System.Windows.Forms;
using ClassRoom.Shared.Models;

namespace ClassRoom.ExamBuilder.Forms
{
    public partial class PreviewForm : Form
    {
        private readonly Assessment _assessment;
        private int _currentIndex = 0;

        public PreviewForm(Assessment assessment)
        {
            _assessment = assessment;
            InitializeComponent();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            LoadQuestion(0);
        }

        private void InitializeComponent()
        {
            this.Text = $"معاينة: {_assessment.Title}";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Font = new Font("Cairo", 10);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(245, 158, 11)
            };

            var lblTitle = new Label
            {
                Text = $"معاينة: {_assessment.Title}",
                Font = new Font("Cairo", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            headerPanel.Controls.Add(lblTitle);

            lblProgress = new Label
            {
                Text = "",
                Font = new Font("Cairo", 11),
                ForeColor = Color.FromArgb(254, 243, 199),
                AutoSize = true,
                Location = new Point(600, 18)
            };
            headerPanel.Controls.Add(lblProgress);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            lblQuestion = new Label
            {
                Text = "",
                Font = new Font("Cairo", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(30, 20),
                MaximumSize = new Size(720, 0)
            };
            contentPanel.Controls.Add(lblQuestion);

            pnlOptions = new Panel
            {
                Location = new Point(30, 100),
                Size = new Size(720, 300),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(pnlOptions);

            btnPrev = new Button
            {
                Text = "السابق",
                Width = 120,
                Height = 40,
                Location = new Point(30, 420),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 11, FontStyle.Bold),
                Enabled = false
            };
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.Click += BtnPrev_Click;
            contentPanel.Controls.Add(btnPrev);

            btnNext = new Button
            {
                Text = "التالي",
                Width = 120,
                Height = 40,
                Location = new Point(170, 420),
                BackColor = Color.FromArgb(14, 165, 233),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 11, FontStyle.Bold)
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += BtnNext_Click;
            contentPanel.Controls.Add(btnNext);

            lblCorrect = new Label
            {
                Text = "",
                Font = new Font("Cairo", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129),
                AutoSize = true,
                Location = new Point(320, 430)
            };
            contentPanel.Controls.Add(lblCorrect);

            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);
        }

        private Label lblProgress = null!;
        private Label lblQuestion = null!;
        private Panel pnlOptions = null!;
        private Button btnPrev = null!;
        private Button btnNext = null!;
        private Label lblCorrect = null!;

        private void LoadQuestion(int index)
        {
            if (index < 0 || index >= _assessment.Questions.Count) return;

            _currentIndex = index;
            var question = _assessment.Questions[index];

            lblQuestion.Text = $"{index + 1}. {question.Text}";
            lblProgress.Text = $"سؤال {index + 1} من {_assessment.Questions.Count}";

            pnlOptions.Controls.Clear();

            for (int i = 0; i < question.Options.Count; i++)
            {
                var radio = new RadioButton
                {
                    Text = question.Options[i],
                    Font = new Font("Cairo", 12),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    AutoSize = true,
                    Location = new Point(10, i * 50),
                    RightToLeft = RightToLeft.Yes,
                    Enabled = false
                };
                pnlOptions.Controls.Add(radio);
            }

            lblCorrect.Text = $"الإجابة الصحيحة: {question.Options[question.CorrectOptionIndex]}";

            btnPrev.Enabled = index > 0;
            btnNext.Enabled = index < _assessment.Questions.Count - 1;
            btnNext.Text = index < _assessment.Questions.Count - 1 ? "التالي" : "إغلاق";
        }

        private void BtnPrev_Click(object? sender, EventArgs e)
        {
            LoadQuestion(_currentIndex - 1);
        }

        private void BtnNext_Click(object? sender, EventArgs e)
        {
            if (_currentIndex < _assessment.Questions.Count - 1)
                LoadQuestion(_currentIndex + 1);
            else
                this.Close();
        }
    }
}
