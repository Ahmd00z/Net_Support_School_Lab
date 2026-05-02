using System;
using System.Collections.Generic;

namespace ClassRoom.Shared.Models
{
    public class Assessment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int TimeLimitMinutes { get; set; } = 30;
        public List<Question> Questions { get; set; } = new();
        public bool IsActive { get; set; } = false;
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public int TotalQuestions => Questions.Count;
        public int TotalMarks => Questions.Count; // 1 mark per question
    }
}
