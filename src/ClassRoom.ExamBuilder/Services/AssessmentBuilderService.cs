using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassRoom.Shared.Models;
using ClassRoom.Shared.Helpers;

namespace ClassRoom.ExamBuilder.Services
{
    public class AssessmentBuilderService
    {
        private Assessment _assessment = new();
        private readonly List<Question> _questions = new();

        public Assessment CurrentAssessment => _assessment;
        public IReadOnlyList<Question> Questions => _questions.AsReadOnly();

        public void NewAssessment(string title, string subject, string description = "", int timeLimit = 30)
        {
            _assessment = new Assessment
            {
                Title = title,
                Subject = subject,
                Description = description,
                TimeLimitMinutes = timeLimit,
                Questions = new List<Question>()
            };
            _questions.Clear();
        }

        public void AddQuestion(string text, List<string> options, int correctIndex)
        {
            var question = new Question
            {
                Text = text,
                Options = options,
                CorrectOptionIndex = correctIndex,
                DisplayOrder = _questions.Count + 1
            };
            _questions.Add(question);
            _assessment.Questions = new List<Question>(_questions);
        }

        public void RemoveQuestion(int index)
        {
            if (index >= 0 && index < _questions.Count)
            {
                _questions.RemoveAt(index);
                _assessment.Questions = new List<Question>(_questions);
                // Reorder
                for (int i = 0; i < _questions.Count; i++)
                    _questions[i].DisplayOrder = i + 1;
            }
        }

        public void UpdateQuestion(int index, string text, List<string> options, int correctIndex)
        {
            if (index >= 0 && index < _questions.Count)
            {
                _questions[index].Text = text;
                _questions[index].Options = options;
                _questions[index].CorrectOptionIndex = correctIndex;
                _assessment.Questions = new List<Question>(_questions);
            }
        }

        public async Task SaveAsync(string folderPath)
        {
            var fileName = JsonDataHelper.GenerateAssessmentFileName(_assessment.Title);
            var filePath = System.IO.Path.Combine(folderPath, fileName);
            await JsonDataHelper.SaveAssessmentAsync(_assessment, filePath);
        }

        public async Task<Assessment?> LoadAsync(string filePath)
        {
            var assessment = await JsonDataHelper.LoadAssessmentAsync(filePath);
            if (assessment != null)
            {
                _assessment = assessment;
                _questions.Clear();
                _questions.AddRange(assessment.Questions);
            }
            return assessment;
        }

        public bool ValidateAssessment()
        {
            if (string.IsNullOrWhiteSpace(_assessment.Title)) return false;
            if (_questions.Count == 0) return false;
            foreach (var q in _questions)
            {
                if (!q.Validate()) return false;
            }
            return true;
        }
    }
}
