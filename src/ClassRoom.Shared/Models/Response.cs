using System;

namespace ClassRoom.Shared.Models
{
    public class Response
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        public string LearnerId { get; set; } = string.Empty;
        public string LearnerName { get; set; } = string.Empty;
        public string AssessmentId { get; set; } = string.Empty;
        public string QuestionId { get; set; } = string.Empty;
        public int SelectedOptionIndex { get; set; } = -1;
        public bool IsCorrect { get; set; } = false;
        public DateTime AnsweredAt { get; set; } = DateTime.Now;
        public int TimeSpentSeconds { get; set; } = 0;
    }
}
