using System;
using System.Drawing;
using System.Windows.Forms;
using ClassRoom.ExamBuilder.Services;
using ClassRoom.Shared.Models;

namespace ClassRoom.ExamBuilder.Forms
{
    public partial class MainForm : Form
    {
        private readonly AssessmentBuilderService _service;
        private int _editingIndex = -1;

        public MainForm()
        {
            _service = new AssessmentBuilderService();
            InitializeComponent();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void InitializeComponent()
        {
            this.Text = "مصمم الاختبارات - ClassRoom Control";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Font = new Font("Cairo", 10);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(139, 92, 246)
            };

            var lblTitle = new Label
            {
                Text = "مصمم الاختبارات",
                Font = new Font("Cairo", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            headerPanel.Controls.Add(lblTitle);

            var toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            btnNew = new Button
            {
                Text = "جديد",
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(14, 165, 233),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(10, 7)
            };
            btnNew.FlatAppearance.BorderSize = 0;
            btnNew.Click += BtnNew_Click;
            toolbarPanel.Controls.Add(btnNew);

            btnLoad = new Button
            {
                Text = "فتح",
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(120, 7)
            };
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Click += BtnLoad_Click;
            toolbarPanel.Controls.Add(btnLoad);

            btnSave = new Button
            {
                Text = "حفظ",
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(230, 7)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            toolbarPanel.Controls.Add(btnSave);

            btnPreview = new Button
            {
                Text = "معاينة",
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(245, 158, 11),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(340, 7)
            };
            btnPreview.FlatAppearance.BorderSize = 0;
            btnPreview.Click += BtnPreview_Click;
            toolbarPanel.Controls.Add(btnPreview);

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 300,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Left - Assessment Info
            var infoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var lblInfoTitle = new Label
            {
                Text = "معلومات الاختبار",
                Font = new Font("Cairo", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            infoPanel.Controls.Add(lblInfoTitle);

            infoPanel.Controls.Add(new Label { Text = "العنوان:", Location = new Point(15, 50), AutoSize = true, Font = new Font("Cairo", 11, FontStyle.Bold) });
            txtTitle = new TextBox { Width = 260, Height = 30, Location = new Point(15, 80), RightToLeft = RightToLeft.Yes, Font = new Font("Cairo", 11) };
            infoPanel.Controls.Add(txtTitle);

            infoPanel.Controls.Add(new Label { Text = "المادة:", Location = new Point(15, 120), AutoSize = true, Font = new Font("Cairo", 11, FontStyle.Bold) });
            txtSubject = new TextBox { Width = 260, Height = 30, Location = new Point(15, 150), RightToLeft = RightToLeft.Yes, Font = new Font("Cairo", 11) };
            infoPanel.Controls.Add(txtSubject);

            infoPanel.Controls.Add(new Label { Text = "الوصف:", Location = new Point(15, 190), AutoSize = true, Font = new Font("Cairo", 11, FontStyle.Bold) });
            txtDescription = new TextBox { Width = 260, Height = 60, Location = new Point(15, 220), RightToLeft = RightToLeft.Yes, Font = new Font("Cairo", 11), Multiline = true };
            infoPanel.Controls.Add(txtDescription);

            infoPanel.Controls.Add(new Label { Text = "الوقت (دقيقة):", Location = new Point(15, 290), AutoSize = true, Font = new Font("Cairo", 11, FontStyle.Bold) });
            numTimeLimit = new NumericUpDown { Width = 100, Height = 30, Location = new Point(15, 320), Minimum = 5, Maximum = 180, Value = 30, Font = new Font("Cairo", 11) };
            infoPanel.Controls.Add(numTimeLimit);

            // Questions List
            infoPanel.Controls.Add(new Label { Text = "الأسئلة:", Location = new Point(15, 360), AutoSize = true, Font = new Font("Cairo", 12, FontStyle.Bold) });

            lbQuestions = new ListBox
            {
                Width = 260,
                Height = 180,
                Location = new Point(15, 390),
                Font = new Font("Cairo", 10),
                RightToLeft = RightToLeft.Yes
            };
            lbQuestions.SelectedIndexChanged += LbQuestions_SelectedIndexChanged;
            infoPanel.Controls.Add(lbQuestions);

            btnDeleteQuestion = new Button
            {
                Text = "حذف سؤال",
                Width = 120,
                Height = 35,
                BackColor = Color.FromArgb(244, 63, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                Location = new Point(155, 580)
            };
            btnDeleteQuestion.FlatAppearance.BorderSize = 0;
            btnDeleteQuestion.Click += BtnDeleteQuestion_Click;
            infoPanel.Controls.Add(btnDeleteQuestion);

            splitContainer.Panel1.Controls.Add(infoPanel);

            // Right - Question Editor
            var editorPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var lblEditorTitle = new Label
            {
                Text = "محرر السؤال",
                Font = new Font("Cairo", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            editorPanel.Controls.Add(lblEditorTitle);

            editorPanel.Controls.Add(new Label { Text = "نص السؤال:", Location = new Point(15, 50), AutoSize = true, Font = new Font("Cairo", 11, FontStyle.Bold) });
            txtQuestionText = new TextBox { Width = 620, Height = 50, Location = new Point(15, 80), RightToLeft = RightToLeft.Yes, Font = new Font("Cairo", 11), Multiline = true };
            editorPanel.Controls.Add(txtQuestionText);

            editorPanel.Controls.Add(new Label { Text = "الخيارات:", Location = new Point(15, 145), AutoSize = true, Font = new Font("Cairo", 11, FontStyle.Bold) });

            txtOption1 = new TextBox { Width = 550, Height = 30, Location = new Point(15, 175), RightToLeft = RightToLeft.Yes, Font = new Font("Cairo", 11) };
            editorPanel.Controls.Add(txtOption1);
            rbOption1 = new RadioButton { Location = new Point(580, 178), Checked = true, Tag = 0 };
            editorPanel.Controls.Add(rbOption1);

            txtOption2 = new TextBox { Width = 550, Height = 30, Location = new Point(15, 215), RightToLeft = RightToLeft.Yes, Font = new Font("Cairo", 11) };
            editorPanel.Controls.Add(txtOption2);
            rbOption2 = new RadioButton { Location = new Point(580, 218), Tag = 1 };
            editorPanel.Controls.Add(rbOption2);

            txtOption3 = new TextBox { Width = 550, Height = 30, Location = new Point(15, 255), RightToLeft = RightToLeft.Yes, Font = new Font("Cairo", 11) };
            editorPanel.Controls.Add(txtOption3);
            rbOption3 = new RadioButton { Location = new Point(580, 258), Tag = 2 };
            editorPanel.Controls.Add(rbOption3);

            txtOption4 = new TextBox { Width = 550, Height = 30, Location = new Point(15, 295), RightToLeft = RightToLeft.Yes, Font = new Font("Cairo", 11) };
            editorPanel.Controls.Add(txtOption4);
            rbOption4 = new RadioButton { Location = new Point(580, 298), Tag = 3 };
            editorPanel.Controls.Add(rbOption4);

            editorPanel.Controls.Add(new Label { Text = "الإجابة الصحيحة:", Location = new Point(15, 340), AutoSize = true, Font = new Font("Cairo", 10), ForeColor = Color.FromArgb(100, 116, 139) });

            btnAddQuestion = new Button
            {
                Text = "إضافة سؤال",
                Width = 150,
                Height = 45,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 12, FontStyle.Bold),
                Location = new Point(15, 380)
            };
            btnAddQuestion.FlatAppearance.BorderSize = 0;
            btnAddQuestion.Click += BtnAddQuestion_Click;
            editorPanel.Controls.Add(btnAddQuestion);

            btnUpdateQuestion = new Button
            {
                Text = "تحديث سؤال",
                Width = 150,
                Height = 45,
                BackColor = Color.FromArgb(14, 165, 233),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 12, FontStyle.Bold),
                Location = new Point(180, 380),
                Visible = false
            };
            btnUpdateQuestion.FlatAppearance.BorderSize = 0;
            btnUpdateQuestion.Click += BtnUpdateQuestion_Click;
            editorPanel.Controls.Add(btnUpdateQuestion);

            btnCancelEdit = new Button
            {
                Text = "إلغاء",
                Width = 100,
                Height = 45,
                BackColor = Color.FromArgb(148, 163, 184),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cairo", 11, FontStyle.Bold),
                Location = new Point(345, 380),
                Visible = false
            };
            btnCancelEdit.FlatAppearance.BorderSize = 0;
            btnCancelEdit.Click += BtnCancelEdit_Click;
            editorPanel.Controls.Add(btnCancelEdit);

            splitContainer.Panel2.Controls.Add(editorPanel);

            this.Controls.Add(splitContainer);
            this.Controls.Add(toolbarPanel);
            this.Controls.Add(headerPanel);
        }

        private Button btnNew = null!, btnLoad = null!, btnSave = null!, btnPreview = null!;
        private Button btnAddQuestion = null!, btnUpdateQuestion = null!, btnCancelEdit = null!, btnDeleteQuestion = null!;
        private TextBox txtTitle = null!, txtSubject = null!, txtDescription = null!, txtQuestionText = null!;
        private TextBox txtOption1 = null!, txtOption2 = null!, txtOption3 = null!, txtOption4 = null!;
        private RadioButton rbOption1 = null!, rbOption2 = null!, rbOption3 = null!, rbOption4 = null!;
        private NumericUpDown numTimeLimit = null!;
        private ListBox lbQuestions = null!;

        private void BtnNew_Click(object? sender, EventArgs e)
        {
            _service.NewAssessment(txtTitle.Text, txtSubject.Text, txtDescription.Text, (int)numTimeLimit.Value);
            RefreshQuestionsList();
            ClearEditor();
            MessageBox.Show("تم إنشاء اختبار جديد", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnLoad_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "فتح اختبار"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var assessment = await _service.LoadAsync(dialog.FileName);
                if (assessment != null)
                {
                    txtTitle.Text = assessment.Title;
                    txtSubject.Text = assessment.Subject;
                    txtDescription.Text = assessment.Description ?? "";
                    numTimeLimit.Value = assessment.TimeLimitMinutes;
                    RefreshQuestionsList();
                    MessageBox.Show("تم تحميل الاختبار بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (!_service.ValidateAssessment())
            {
                MessageBox.Show("الرجاء إدخال عنوان وأسئلة للاختبار", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dialog = new FolderBrowserDialog { Description = "اختر مجلد الحفظ" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                await _service.SaveAsync(dialog.SelectedPath);
                MessageBox.Show("تم حفظ الاختبار بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnPreview_Click(object? sender, EventArgs e)
        {
            if (_service.Questions.Count == 0)
            {
                MessageBox.Show("لا يوجد أسئلة للمعاينة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var preview = new PreviewForm(_service.CurrentAssessment);
            preview.ShowDialog();
        }

        private void BtnAddQuestion_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuestionText.Text))
            {
                MessageBox.Show("الرجاء إدخال نص السؤال", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var options = new List<string>
            {
                txtOption1.Text,
                txtOption2.Text,
                txtOption3.Text,
                txtOption4.Text
            };

            if (options.Any(string.IsNullOrWhiteSpace))
            {
                MessageBox.Show("الرجاء إدخال جميع الخيارات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int correctIndex = GetSelectedCorrectIndex();
            _service.AddQuestion(txtQuestionText.Text, options, correctIndex);
            RefreshQuestionsList();
            ClearEditor();
        }

        private void BtnUpdateQuestion_Click(object? sender, EventArgs e)
        {
            if (_editingIndex < 0) return;

            var options = new List<string>
            {
                txtOption1.Text,
                txtOption2.Text,
                txtOption3.Text,
                txtOption4.Text
            };

            _service.UpdateQuestion(_editingIndex, txtQuestionText.Text, options, GetSelectedCorrectIndex());
            RefreshQuestionsList();
            ClearEditor();
            EndEditMode();
        }

        private void BtnCancelEdit_Click(object? sender, EventArgs e)
        {
            ClearEditor();
            EndEditMode();
        }

        private void BtnDeleteQuestion_Click(object? sender, EventArgs e)
        {
            if (lbQuestions.SelectedIndex < 0) return;
            _service.RemoveQuestion(lbQuestions.SelectedIndex);
            RefreshQuestionsList();
        }

        private void LbQuestions_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lbQuestions.SelectedIndex < 0) return;
            var question = _service.Questions[lbQuestions.SelectedIndex];
            txtQuestionText.Text = question.Text;
            txtOption1.Text = question.Options[0];
            txtOption2.Text = question.Options[1];
            txtOption3.Text = question.Options[2];
            txtOption4.Text = question.Options[3];
            SetSelectedCorrectIndex(question.CorrectOptionIndex);
            StartEditMode(lbQuestions.SelectedIndex);
        }

        private void RefreshQuestionsList()
        {
            lbQuestions.Items.Clear();
            foreach (var q in _service.Questions)
            {
                lbQuestions.Items.Add($"{q.DisplayOrder}. {q.Text}");
            }
        }

        private void ClearEditor()
        {
            txtQuestionText.Clear();
            txtOption1.Clear();
            txtOption2.Clear();
            txtOption3.Clear();
            txtOption4.Clear();
            rbOption1.Checked = true;
        }

        private void StartEditMode(int index)
        {
            _editingIndex = index;
            btnAddQuestion.Visible = false;
            btnUpdateQuestion.Visible = true;
            btnCancelEdit.Visible = true;
        }

        private void EndEditMode()
        {
            _editingIndex = -1;
            btnAddQuestion.Visible = true;
            btnUpdateQuestion.Visible = false;
            btnCancelEdit.Visible = false;
        }

        private int GetSelectedCorrectIndex()
        {
            if (rbOption1.Checked) return 0;
            if (rbOption2.Checked) return 1;
            if (rbOption3.Checked) return 2;
            return 3;
        }

        private void SetSelectedCorrectIndex(int index)
        {
            rbOption1.Checked = index == 0;
            rbOption2.Checked = index == 1;
            rbOption3.Checked = index == 2;
            rbOption4.Checked = index == 3;
        }
    }
}
