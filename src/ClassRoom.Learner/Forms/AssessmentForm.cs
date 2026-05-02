using System;
using System.Drawing;
using System.Windows.Forms;
using ClassRoom.Learner.Services;
using ClassRoom.Shared.Models;

namespace ClassRoom.Learner.Forms
{
    public partial class AssessmentForm : Form
    {
        private readonly LearnerService _service;
        private readonly Assessment _assessment;
        private readonly string _learnerName;
        private int _currentQuestionIndex = 0;
        private int? _selectedOption = null;
        private readonly System.Diagnostics.Stopwatch _stopwatch = new();

        public AssessmentForm(LearnerService service, Assessment assessment, string learnerName)
        {
            _service = service;
            _assessment = assessment;
            _learnerName = learnerName;
            InitializeComponent();
            LoadQuestion(0);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void InitializeComponent()
        {
            this.Text = $"اختبار: {_assessment.Title}";
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
                BackColor = Color.FromArgb(139, 92, 246)
            };

            lblTitle = new Label
            {
                Text = _assessment.Title,
                Font = new Font("Cairo", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            headerPanel.Controls.Add(lblTitle);

            lblProgress = new Label
            {
                Text = $"سؤال 1 من {_assessment.Questions.Count}",
                Font = new Font("Cairo", 11),
                ForeColor = Color.FromArgb(221, 214, 254),
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

            btnNext = new Button
            {
                Text = "التالي",
                Width = 150,
                Height = 45,
                Location = new Point(600, 420),
                BackColor = Color.FromArgb(14, 165, 233),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 12, FontStyle.Bold),
                Enabled = false
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += BtnNext_Click;
            contentPanel.Controls.Add(btnNext);

            btnSubmit = new Button
            {
                Text = "إرسال الإجابات",
                Width = 150,
                Height = 45,
                Location = new Point(430, 420),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 12, FontStyle.Bold),
                Visible = false
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += BtnSubmit_Click;
            contentPanel.Controls.Add(btnSubmit);

            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);

            _stopwatch.Start();
        }

        private Label lblTitle = null!;
        private Label lblProgress = null!;
        private Label lblQuestion = null!;
        private Panel pnlOptions = null!;
        private Button btnNext = null!;
        private Button btnSubmit = null!;

        private void LoadQuestion(int index)
        {
            if (index < 0 || index >= _assessment.Questions.Count) return;

            _currentQuestionIndex = index;
            var question = _assessment.Questions[index];

            lblQuestion.Text = $"{index + 1}. {question.Text}";
            lblProgress.Text = $"سؤال {index + 1} من {_assessment.Questions.Count}";

            pnlOptions.Controls.Clear();
            _selectedOption = null;
            btnNext.Enabled = false;

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
                    Tag = i
                };
                radio.CheckedChanged += (s, e) =>
                {
                    if (radio.Checked)
                    {
                        _selectedOption = (int)radio.Tag;
                        btnNext.Enabled = true;
                    }
                };
                pnlOptions.Controls.Add(radio);
            }

            if (index == _assessment.Questions.Count - 1)
            {
                btnNext.Visible = false;
                btnSubmit.Visible = true;
            }
        }

        private async void BtnNext_Click(object? sender, EventArgs e)
        {
            await SubmitCurrentResponse();
            LoadQuestion(_currentQuestionIndex + 1);
        }

        private async void BtnSubmit_Click(object? sender, EventArgs e)
        {
            await SubmitCurrentResponse();
            _stopwatch.Stop();
            MessageBox.Show("تم إرسال إجاباتك بنجاح!", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private async Task SubmitCurrentResponse()
        {
            if (_selectedOption == null) return;

            var question = _assessment.Questions[_currentQuestionIndex];
            bool isCorrect = _selectedOption.Value == question.CorrectOptionIndex;

            await _service.SubmitResponseAsync(
                question.Id,
                _selectedOption.Value,
                isCorrect,
                (int)_stopwatch.Elapsed.TotalSeconds
            );
        }
    }
}
