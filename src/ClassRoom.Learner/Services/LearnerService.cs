using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ClassRoom.Shared.Models;
using ClassRoom.Shared.Hubs;

namespace ClassRoom.Learner.Services
{
    public class LearnerService
    {
        private string? _learnerId;
        private Assessment? _currentAssessment;
        private readonly System.Timers.Timer _refreshTimer;
        private bool _isRunning = false;

        public event EventHandler<bool>? ConnectionStateChanged;
        public event EventHandler? LockReceived;
        public event EventHandler? UnlockReceived;
        public event EventHandler<Assessment>? AssessmentReceived;
        public event EventHandler? AssessmentEnded;
        public event EventHandler<string>? IdConfirmed;

        public bool IsConnected => _isRunning;
        public string? LearnerId => _learnerId;
        public Assessment? CurrentAssessment => _currentAssessment;

        public LearnerService()
        {
            _refreshTimer = new System.Timers.Timer(1500); // Check every 1.5 seconds
            _refreshTimer.Elapsed += async (s, e) => await RefreshDataAsync();
        }

        public async Task ConnectAsync(string learnerName, string classroomCode)
        {
            _learnerId = await ClassRoomHub.JoinClassroom(learnerName, classroomCode);
            _isRunning = true;
            IdConfirmed?.Invoke(this, _learnerId);
            ConnectionStateChanged?.Invoke(this, true);
            _refreshTimer.Start();
        }

        public async Task DisconnectAsync()
        {
            _isRunning = false;
            _refreshTimer.Stop();
            if (_learnerId != null)
                await ClassRoomHub.LeaveClassroom(_learnerId);
            ConnectionStateChanged?.Invoke(this, false);
        }

        private async Task RefreshDataAsync()
        {
            if (!_isRunning || _learnerId == null) return;

            try
            {
                // Check for assessment
                var assessment = await ClassRoomHub.GetActiveAssessmentAsync();
                if (assessment != null && _currentAssessment?.Id != assessment.Id)
                {
                    _currentAssessment = assessment;
                    AssessmentReceived?.Invoke(this, assessment);
                }
                else if (assessment == null && _currentAssessment != null)
                {
                    _currentAssessment = null;
                    AssessmentEnded?.Invoke(this, EventArgs.Empty);
                }

                // Check for commands
                var commands = await ClassRoomHub.GetCommandsForLearnerAsync(_learnerId);
                foreach (var cmd in commands.OrderBy(c => c.Timestamp))
                {
                    switch (cmd.Type)
                    {
                        case "lock":
                            LockReceived?.Invoke(this, EventArgs.Empty);
                            break;
                        case "unlock":
                            UnlockReceived?.Invoke(this, EventArgs.Empty);
                            break;
                    }
                }
            }
            catch { /* Ignore refresh errors */ }
        }

        public async Task SubmitResponseAsync(string questionId, int selectedOptionIndex, bool isCorrect, int timeSpentSeconds)
        {
            if (_learnerId == null) return;

            var response = new Response
            {
                LearnerId = _learnerId,
                AssessmentId = _currentAssessment?.Id ?? "",
                QuestionId = questionId,
                SelectedOptionIndex = selectedOptionIndex,
                IsCorrect = isCorrect,
                TimeSpentSeconds = timeSpentSeconds
            };

            await ClassRoomHub.SubmitResponse(response);
        }
    }
}
