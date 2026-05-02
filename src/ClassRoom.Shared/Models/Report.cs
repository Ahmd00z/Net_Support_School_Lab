using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassRoom.Shared.Models
{
    public class Report
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        public string LearnerId { get; set; } = string.Empty;
        public string LearnerName { get; set; } = string.Empty;
        public string AssessmentId { get; set; } = string.Empty;
        public string AssessmentTitle { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public List<Response> Responses { get; set; } = new();

        public int TotalQuestions { get; set; }
        public int CorrectAnswers => Responses.Count(r => r.IsCorrect);
        public int WrongAnswers => Responses.Count(r => !r.IsCorrect && r.SelectedOptionIndex >= 0);
        public int Unanswered => TotalQuestions - Responses.Count(r => r.SelectedOptionIndex >= 0);
        public double ScorePercentage => TotalQuestions > 0 
            ? Math.Round((double)CorrectAnswers / TotalQuestions * 100, 2) 
            : 0;
        public string Grade => ScorePercentage switch
        {
            >= 90 => "ممتاز",
            >= 80 => "جيد جداً",
            >= 70 => "جيد",
            >= 60 => "مقبول",
            _ => "ضعيف"
        };
        public TimeSpan TotalTimeSpent { get; set; }
    }
}
