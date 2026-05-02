using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ClassRoom.Shared.Models;
using ClassRoom.Shared.Helpers;
using ClassRoom.Shared.Hubs;

namespace ClassRoom.Instructor.Services
{
    public class InstructorService
    {
        private readonly List<Learner> _learners = new();
        private Assessment? _currentAssessment;
        private readonly Dictionary<string, List<Response>> _learnerResponses = new();
        private readonly System.Timers.Timer _refreshTimer;
        private bool _isRunning = false;

        public event EventHandler<Learner>? LearnerJoined;
        public event EventHandler<string>? LearnerLeft;
        public event EventHandler<(string, LearnerStatus)>? StatusChanged;
        public event EventHandler<Response>? ResponseReceived;
        public event EventHandler? AssessmentStarted;
        public event EventHandler? AssessmentEnded;
        public event EventHandler<bool>? ConnectionStateChanged;

        public bool IsConnected => _isRunning;
        public string? ClassroomCode { get; private set; }
        public IReadOnlyList<Learner> Learners => _learners.AsReadOnly();
        public Assessment? CurrentAssessment => _currentAssessment;

        public InstructorService()
        {
            _refreshTimer = new System.Timers.Timer(2000); // Check every 2 seconds
            _refreshTimer.Elapsed += async (s, e) => await RefreshDataAsync();
        }

        public async Task ConnectAsync(string instructorName, string classroomCode)
        {
            ClassroomCode = classroomCode;
            _isRunning = true;

            // Clear old data
            await ClassRoomHub.ClearAllAsync();

            _refreshTimer.Start();
            await RefreshDataAsync();
            ConnectionStateChanged?.Invoke(this, true);
        }

        public async Task DisconnectAsync()
        {
            _isRunning = false;
            _refreshTimer.Stop();
            await ClassRoomHub.ClearAllAsync();
            ConnectionStateChanged?.Invoke(this, false);
        }

        private async Task RefreshDataAsync()
        {
            if (!_isRunning) return;

            try
            {
                var hubLearners = await ClassRoomHub.GetLearnersAsync();

                // Detect new learners
                foreach (var hubLearner in hubLearners)
                {
                    if (!_learners.Any(l => l.Id == hubLearner.Id))
                    {
                        _learners.Add(hubLearner);
                        LearnerJoined?.Invoke(this, hubLearner);
                    }
                    else
                    {
                        var existing = _learners.First(l => l.Id == hubLearner.Id);
                        if (existing.Status != hubLearner.Status)
                        {
                            existing.Status = hubLearner.Status;
                            StatusChanged?.Invoke(this, (hubLearner.Id, hubLearner.Status));
                        }
                    }
                }

                // Detect removed learners
                var toRemove = _learners.Where(l => !hubLearners.Any(hl => hl.Id == l.Id)).Select(l => l.Id).ToList();
                foreach (var id in toRemove)
                {
                    _learners.RemoveAll(l => l.Id == id);
                    LearnerLeft?.Invoke(this, id);
                }

                // Check for responses
                var responses = await ClassRoomHub.GetResponsesAsync();
                foreach (var response in responses)
                {
                    if (!_learnerResponses.ContainsKey(response.LearnerId))
                        _learnerResponses[response.LearnerId] = new List<Response>();

                    if (!_learnerResponses[response.LearnerId].Any(r => r.Id == response.Id))
                    {
                        _learnerResponses[response.LearnerId].Add(response);
                        ResponseReceived?.Invoke(this, response);
                    }
                }
            }
            catch { /* Ignore refresh errors */ }
        }

        public async Task LockLearnerAsync(string learnerId)
        {
            await ClassRoomHub.SendLock(learnerId);
        }

        public async Task UnlockLearnerAsync(string learnerId)
        {
            await ClassRoomHub.SendUnlock(learnerId);
        }

        public async Task StartAssessmentAsync(Assessment assessment)
        {
            _currentAssessment = assessment;
            await ClassRoomHub.StartAssessment(assessment);
            AssessmentStarted?.Invoke(this, EventArgs.Empty);
        }

        public async Task EndAssessmentAsync()
        {
            await ClassRoomHub.EndAssessment();
            AssessmentEnded?.Invoke(this, EventArgs.Empty);
            _currentAssessment = null;
        }

        public Report GenerateReport(string learnerId)
        {
            var learner = _learners.Find(l => l.Id == learnerId);
            if (learner == null || _currentAssessment == null)
                throw new InvalidOperationException("Learner or assessment not found");

            var responses = _learnerResponses.ContainsKey(learnerId) 
                ? _learnerResponses[learnerId] 
                : new List<Response>();

            return new Report
            {
                LearnerId = learnerId,
                LearnerName = learner.Name,
                AssessmentId = _currentAssessment.Id,
                AssessmentTitle = _currentAssessment.Title,
                TotalQuestions = _currentAssessment.TotalQuestions,
                Responses = responses,
                TotalTimeSpent = _currentAssessment.StartedAt.HasValue 
                    ? DateTime.Now - _currentAssessment.StartedAt.Value 
                    : TimeSpan.Zero
            };
        }

        public async Task ExportReportAsync(Report report, string folderPath)
        {
            var fileName = JsonDataHelper.GenerateReportFileName(report.LearnerName, report.AssessmentTitle);
            var filePath = System.IO.Path.Combine(folderPath, fileName);
            await JsonDataHelper.SaveReportAsync(report, filePath);
        }
    }
}
