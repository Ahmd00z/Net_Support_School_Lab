using System;
using System.Collections.Generic;

namespace ClassRoom.Shared.Models
{
    public class Instructor
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        public string Name { get; set; } = string.Empty;
        public string ClassroomCode { get; set; } = $"ROOM-{new Random().Next(100, 999)}";
        public DateTime SessionStarted { get; set; } = DateTime.Now;
        public List<Learner> ConnectedLearners { get; set; } = new();
        public Assessment? ActiveAssessment { get; set; }
        public bool IsSessionActive { get; set; } = true;
    }
}
