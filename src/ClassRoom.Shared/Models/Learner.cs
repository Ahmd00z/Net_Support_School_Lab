using System;

namespace ClassRoom.Shared.Models
{
    public class Learner
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        public string Name { get; set; } = string.Empty;
        public string ClassroomCode { get; set; } = string.Empty;
        public LearnerStatus Status { get; set; } = LearnerStatus.Online;
        public DateTime ConnectedAt { get; set; } = DateTime.Now;
        public DateTime? LastActivity { get; set; }
        public string? CurrentAssessmentId { get; set; }
        public int? CurrentScore { get; set; }

        public override string ToString() => $"{Name} ({Id})";
    }

    public enum LearnerStatus
    {
        Offline,
        Online,
        Locked,
        InAssessment,
        Completed
    }
}
