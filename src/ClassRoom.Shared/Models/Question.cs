using System;
using System.Collections.Generic;

namespace ClassRoom.Shared.Models
{
    public class Question
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectOptionIndex { get; set; } = 0;
        public int Marks { get; set; } = 1;
        public string? ImagePath { get; set; }
        public int DisplayOrder { get; set; } = 0;

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Text) 
                && Options.Count >= 2 
                && CorrectOptionIndex >= 0 
                && CorrectOptionIndex < Options.Count;
        }
    }
}
