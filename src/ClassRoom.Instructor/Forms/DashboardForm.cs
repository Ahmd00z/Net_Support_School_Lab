using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClassRoom.Instructor.Services;
using ClassRoom.Shared.Models;
using ClassRoom.Shared.Helpers;

namespace ClassRoom.Instructor.Forms
{
    public partial class DashboardForm : Form
    {
        private readonly InstructorService _service;
        private Assessment? _loadedAssessment;

        public DashboardForm()
        {
            _service = new InstructorService();
            InitializeComponent();
            SetupEvents();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void SetupEvents()
        {
            _service.LearnerJoined += (s, learner) =>
            {
                Invoke(() => AddLearnerToGrid(learner));
            };

            _service.LearnerLeft += (s, learnerId) =>
            {
                Invoke(() => RemoveLearnerFromGrid(learnerId));
            };

            _service.StatusChanged += (s, args) =>
            {
                Invoke(() => UpdateLearnerStatus(args.Item1, args.Item2));
            };

            _service.ResponseReceived += (s, response) =>
            {
                Invoke(() => UpdateResponseCount(response.LearnerId));
            };

            _service.ConnectionStateChanged += (s, connected) =>
            {
                Invoke(() => lblConnectionStatus.Text = connected ? "متصل" : "غير متصل");
                Invoke(() => lblConnectionStatus.ForeColor = connected ? Color.FromArgb(16, 185, 129) : Color.FromArgb(244, 63, 94));
            };
        }

        private void InitializeComponent()
        {
            this.Text = "لوحة تحكم المدرس - ClassRoom Control";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Font = new Font("Cairo", 10);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(14, 165, 233)
            };

            var lblTitle = new Label
            {
                Text = "لوحة تحكم المدرس",
                Font = new Font("Cairo", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            headerPanel.Controls.Add(lblTitle);

            lblConnectionStatus = new Label
            {
                Text = "غير متصل",
                Font = new Font("Cairo", 10),
                ForeColor = Color.FromArgb(244, 63, 94),
                AutoSize = true,
                Location = new Point(950, 25)
            };
            headerPanel.Controls.Add(lblConnectionStatus);

            var connPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            txtInstructorName = new TextBox
            {
                Width = 180,
                Height = 30,
                RightToLeft = RightToLeft.Yes,
                Font = new Font("Cairo", 10),
                Location = new Point(850, 15)
            };
            connPanel.Controls.Add(new Label { Text = "اسم المدرس:", Location = new Point(1040, 18), AutoSize = true, Font = new Font("Cairo", 10) });
            connPanel.Controls.Add(txtInstructorName);

            txtClassroomCode = new TextBox
            {
                Width = 150,
                Height = 30,
                Text = "ROOM-101",
                RightToLeft = RightToLeft.Yes,
                Font = new Font("Cairo", 10),
                Location = new Point(650, 15)
            };
            connPanel.Controls.Add(new Label { Text = "كود الفصل:", Location = new Point(810, 18), AutoSize = true, Font = new Font("Cairo", 10) });
            connPanel.Controls.Add(txtClassroomCode);

            btnConnect = new Button
            {
                Text = "اتصال",
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(14, 165, 233),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(530, 12)
            };
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.Click += BtnConnect_Click;
            connPanel.Controls.Add(btnConnect);

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 350,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            var learnersPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var lblLearners = new Label
            {
                Text = "قائمة الطلاب",
                Font = new Font("Cairo", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            learnersPanel.Controls.Add(lblLearners);

            dgvLearners = new DataGridView
            {
                Location = new Point(15, 45),
                Size = new Size(320, 450),
                RightToLeft = RightToLeft.Yes,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(14, 165, 233),
                    ForeColor = Color.White,
                    Font = new Font("Cairo", 10, FontStyle.Bold)
                },
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvLearners.Columns.Add("Id", "الكود");
            dgvLearners.Columns.Add("Name", "الاسم");
            dgvLearners.Columns.Add("Status", "الحالة");
            dgvLearners.Columns.Add("Responses", "الإجابات");
            learnersPanel.Controls.Add(dgvLearners);

            btnLock = new Button
            {
                Text = "قفل الشاشة",
                Width = 150,
                Height = 40,
                BackColor = Color.FromArgb(244, 63, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(185, 510)
            };
            btnLock.FlatAppearance.BorderSize = 0;
            btnLock.Click += BtnLock_Click;
            learnersPanel.Controls.Add(btnLock);

            btnUnlock = new Button
            {
                Text = "فتح الشاشة",
                Width = 150,
                Height = 40,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(15, 510)
            };
            btnUnlock.FlatAppearance.BorderSize = 0;
            btnUnlock.Click += BtnUnlock_Click;
            learnersPanel.Controls.Add(btnUnlock);

            splitContainer.Panel1.Controls.Add(learnersPanel);

            var assessmentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var lblAssessment = new Label
            {
                Text = "التحكم في الاختبار",
                Font = new Font("Cairo", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            assessmentPanel.Controls.Add(lblAssessment);

            btnLoadAssessment = new Button
            {
                Text = "تحميل اختبار",
                Width = 150,
                Height = 40,
                BackColor = Color.FromArgb(139, 92, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(15, 50)
            };
            btnLoadAssessment.FlatAppearance.BorderSize = 0;
            btnLoadAssessment.Click += BtnLoadAssessment_Click;
            assessmentPanel.Controls.Add(btnLoadAssessment);

            lblAssessmentName = new Label
            {
                Text = "لا يوجد اختبار محمل",
                Font = new Font("Cairo", 11),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(180, 60)
            };
            assessmentPanel.Controls.Add(lblAssessmentName);

            btnStartAssessment = new Button
            {
                Text = "بدء الاختبار",
                Width = 150,
                Height = 45,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 11, FontStyle.Bold),
                Location = new Point(15, 110),
                Enabled = false
            };
            btnStartAssessment.FlatAppearance.BorderSize = 0;
            btnStartAssessment.Click += BtnStartAssessment_Click;
            assessmentPanel.Controls.Add(btnStartAssessment);

            btnEndAssessment = new Button
            {
                Text = "إنهاء الاختبار",
                Width = 150,
                Height = 45,
                BackColor = Color.FromArgb(244, 63, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 11, FontStyle.Bold),
                Location = new Point(180, 110),
                Enabled = false
            };
            btnEndAssessment.FlatAppearance.BorderSize = 0;
            btnEndAssessment.Click += BtnEndAssessment_Click;
            assessmentPanel.Controls.Add(btnEndAssessment);

            var lblReport = new Label
            {
                Text = "التقارير",
                Font = new Font("Cairo", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(15, 180)
            };
            assessmentPanel.Controls.Add(lblReport);

            btnViewReport = new Button
            {
                Text = "عرض تقرير",
                Width = 150,
                Height = 40,
                BackColor = Color.FromArgb(14, 165, 233),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(15, 220)
            };
            btnViewReport.FlatAppearance.BorderSize = 0;
            btnViewReport.Click += BtnViewReport_Click;
            assessmentPanel.Controls.Add(btnViewReport);

            btnExportReport = new Button
            {
                Text = "تصدير تقرير",
                Width = 150,
                Height = 40,
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(180, 220)
            };
            btnExportReport.FlatAppearance.BorderSize = 0;
            btnExportReport.Click += BtnExportReport_Click;
            assessmentPanel.Controls.Add(btnExportReport);

            txtReportPreview = new TextBox
            {
                Location = new Point(15, 280),
                Size = new Size(680, 250),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                RightToLeft = RightToLeft.Yes,
                Font = new Font("Cairo", 10),
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle
            };
            assessmentPanel.Controls.Add(txtReportPreview);

            splitContainer.Panel2.Controls.Add(assessmentPanel);

            this.Controls.Add(splitContainer);
            this.Controls.Add(connPanel);
            this.Controls.Add(headerPanel);
        }

        private TextBox txtInstructorName = null!;
        private TextBox txtClassroomCode = null!;
        private Button btnConnect = null!;
        private Label lblConnectionStatus = null!;
        private DataGridView dgvLearners = null!;
        private Button btnLock = null!;
        private Button btnUnlock = null!;
        private Button btnLoadAssessment = null!;
        private Label lblAssessmentName = null!;
        private Button btnStartAssessment = null!;
        private Button btnEndAssessment = null!;
        private Button btnViewReport = null!;
        private Button btnExportReport = null!;
        private TextBox txtReportPreview = null!;

        private async void BtnConnect_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtInstructorName.Text))
                {
                    MessageBox.Show("الرجاء إدخال اسم المدرس", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                await _service.ConnectAsync(txtInstructorName.Text, txtClassroomCode.Text);
                btnConnect.Text = "متصل";
                btnConnect.Enabled = false;
                txtInstructorName.Enabled = false;
                txtClassroomCode.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"فشل الاتصال: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddLearnerToGrid(Learner learner)
        {
            dgvLearners.Rows.Add(learner.Id, learner.Name, GetStatusText(learner.Status), "0");
        }

        private void RemoveLearnerFromGrid(string learnerId)
        {
            foreach (DataGridViewRow row in dgvLearners.Rows)
            {
                if (row.Cells["Id"].Value?.ToString() == learnerId)
                {
                    dgvLearners.Rows.Remove(row);
                    break;
                }
            }
        }

        private void UpdateLearnerStatus(string learnerId, LearnerStatus status)
        {
            foreach (DataGridViewRow row in dgvLearners.Rows)
            {
                if (row.Cells["Id"].Value?.ToString() == learnerId)
                {
                    row.Cells["Status"].Value = GetStatusText(status);
                    break;
                }
            }
        }

        private void UpdateResponseCount(string learnerId)
        {
            foreach (DataGridViewRow row in dgvLearners.Rows)
            {
                if (row.Cells["Id"].Value?.ToString() == learnerId)
                {
                    int current = int.Parse(row.Cells["Responses"].Value?.ToString() ?? "0");
                    row.Cells["Responses"].Value = (current + 1).ToString();
                    break;
                }
            }
        }

        private string GetStatusText(LearnerStatus status) => status switch
        {
            LearnerStatus.Online => "متصل",
            LearnerStatus.Offline => "غير متصل",
            LearnerStatus.Locked => "مقفل",
            LearnerStatus.InAssessment => "في الاختبار",
            LearnerStatus.Completed => "مكتمل",
            _ => "غير معروف"
        };

        private async void BtnLock_Click(object? sender, EventArgs e)
        {
            if (dgvLearners.SelectedRows.Count == 0) return;
            var learnerId = dgvLearners.SelectedRows[0].Cells["Id"].Value?.ToString();
            if (learnerId != null)
                await _service.LockLearnerAsync(learnerId);
        }

        private async void BtnUnlock_Click(object? sender, EventArgs e)
        {
            if (dgvLearners.SelectedRows.Count == 0) return;
            var learnerId = dgvLearners.SelectedRows[0].Cells["Id"].Value?.ToString();
            if (learnerId != null)
                await _service.UnlockLearnerAsync(learnerId);
        }

        private async void BtnLoadAssessment_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "اختر ملف الاختبار",
                InitialDirectory = System.IO.Path.Combine(Application.StartupPath, "samples", "exams")
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _loadedAssessment = await JsonDataHelper.LoadAssessmentAsync(dialog.FileName);
                if (_loadedAssessment != null)
                {
                    lblAssessmentName.Text = _loadedAssessment.Title;
                    lblAssessmentName.ForeColor = Color.FromArgb(51, 65, 85);
                    btnStartAssessment.Enabled = true;
                }
            }
        }

        private async void BtnStartAssessment_Click(object? sender, EventArgs e)
        {
            if (_loadedAssessment == null) return;
            await _service.StartAssessmentAsync(_loadedAssessment);
            btnStartAssessment.Enabled = false;
            btnEndAssessment.Enabled = true;
            MessageBox.Show("تم بدء الاختبار بنجاح!", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnEndAssessment_Click(object? sender, EventArgs e)
        {
            await _service.EndAssessmentAsync();
            btnStartAssessment.Enabled = true;
            btnEndAssessment.Enabled = false;
            MessageBox.Show("تم إنهاء الاختبار!", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnViewReport_Click(object? sender, EventArgs e)
        {
            if (dgvLearners.SelectedRows.Count == 0)
            {
                MessageBox.Show("الرجاء اختيار طالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var learnerId = dgvLearners.SelectedRows[0].Cells["Id"].Value?.ToString();
            if (learnerId == null) return;
            try
            {
                var report = _service.GenerateReport(learnerId);
                txtReportPreview.Text = $"تقرير الطالب: {report.LearnerName}\r\n" +
                                       $"الاختبار: {report.AssessmentTitle}\r\n" +
                                       $"الإجابات الصحيحة: {report.CorrectAnswers} من {report.TotalQuestions}\r\n" +
                                       $"النسبة: {report.ScorePercentage}%\r\n" +
                                       $"التقدير: {report.Grade}\r\n" +
                                       $"الوقت المستغرق: {report.TotalTimeSpent.Minutes} دقيقة {report.TotalTimeSpent.Seconds} ثانية\r\n" +
                                       $"تاريخ التقرير: {report.GeneratedAt:yyyy/MM/dd HH:mm}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إنشاء التقرير: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnExportReport_Click(object? sender, EventArgs e)
        {
            if (dgvLearners.SelectedRows.Count == 0) return;
            var learnerId = dgvLearners.SelectedRows[0].Cells["Id"].Value?.ToString();
            if (learnerId == null) return;
            using var dialog = new FolderBrowserDialog { Description = "اختر مجلد حفظ التقرير" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var report = _service.GenerateReport(learnerId);
                    await _service.ExportReportAsync(report, dialog.SelectedPath);
                    MessageBox.Show("تم تصدير التقرير بنجاح!", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ في التصدير: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            await _service.DisconnectAsync();
            base.OnFormClosing(e);
        }
    }
}
