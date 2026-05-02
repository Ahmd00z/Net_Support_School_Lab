using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClassRoom.Shared.Models;
using ClassRoom.Shared.Helpers;

namespace ClassRoom.Shared.Hubs
{
    /// <summary>
    /// Simple file-based hub for local communication (no SignalR server needed)
    /// </summary>
    public class ClassRoomHub
    {
        private static readonly string _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hub_data");
        private static readonly string _learnersFile = Path.Combine(_basePath, "learners.json");
        private static readonly string _commandsFile = Path.Combine(_basePath, "commands.json");
        private static readonly string _responsesFile = Path.Combine(_basePath, "responses.json");
        private static readonly string _assessmentFile = Path.Combine(_basePath, "active_assessment.json");

        static ClassRoomHub()
        {
            Directory.CreateDirectory(_basePath);
        }

        // ===== Learner Methods =====
        public static async Task<string> JoinClassroom(string learnerName, string classroomCode)
        {
            var learner = new Learner
            {
                Name = learnerName,
                ClassroomCode = classroomCode,
                Status = LearnerStatus.Online
            };

            var learners = await LoadLearnersAsync();
            learners.RemoveAll(l => l.Name == learnerName);
            learners.Add(learner);
            await SaveLearnersAsync(learners);

            return learner.Id;
        }

        public static async Task LeaveClassroom(string learnerId)
        {
            var learners = await LoadLearnersAsync();
            learners.RemoveAll(l => l.Id == learnerId);
            await SaveLearnersAsync(learners);
        }

        public static async Task SubmitResponse(Response response)
        {
            var responses = await LoadResponsesAsync();
            responses.Add(response);
            await SaveResponsesAsync(responses);
        }

        // ===== Instructor Methods =====
        public static async Task<List<Learner>> GetLearnersAsync()
        {
            return await LoadLearnersAsync();
        }

        public static async Task SendLock(string learnerId)
        {
            await AddCommandAsync(new HubCommand 
            { 
                Type = "lock", 
                TargetLearnerId = learnerId, 
                Timestamp = DateTime.Now 
            });

            var learners = await LoadLearnersAsync();
            var learner = learners.FirstOrDefault(l => l.Id == learnerId);
            if (learner != null)
            {
                learner.Status = LearnerStatus.Locked;
                await SaveLearnersAsync(learners);
            }
        }

        public static async Task SendUnlock(string learnerId)
        {
            await AddCommandAsync(new HubCommand 
            { 
                Type = "unlock", 
                TargetLearnerId = learnerId, 
                Timestamp = DateTime.Now 
            });

            var learners = await LoadLearnersAsync();
            var learner = learners.FirstOrDefault(l => l.Id == learnerId);
            if (learner != null)
            {
                learner.Status = LearnerStatus.Online;
                await SaveLearnersAsync(learners);
            }
        }

        public static async Task StartAssessment(Assessment assessment)
        {
            await JsonDataHelper.SaveAssessmentAsync(assessment, _assessmentFile);

            var learners = await LoadLearnersAsync();
            foreach (var l in learners)
            {
                l.Status = LearnerStatus.InAssessment;
            }
            await SaveLearnersAsync(learners);
        }

        public static async Task EndAssessment()
        {
            if (File.Exists(_assessmentFile))
                File.Delete(_assessmentFile);

            var learners = await LoadLearnersAsync();
            foreach (var l in learners)
            {
                l.Status = LearnerStatus.Completed;
            }
            await SaveLearnersAsync(learners);
        }

        public static async Task<Assessment?> GetActiveAssessmentAsync()
        {
            if (!File.Exists(_assessmentFile)) return null;
            return await JsonDataHelper.LoadAssessmentAsync(_assessmentFile);
        }

        public static async Task<List<HubCommand>> GetCommandsForLearnerAsync(string learnerId)
        {
            var commands = await LoadCommandsAsync();
            var learnerCommands = commands.Where(c => c.TargetLearnerId == learnerId || c.TargetLearnerId == "all").ToList();

            // Remove processed commands older than 5 minutes
            var cutoff = DateTime.Now.AddMinutes(-5);
            commands.RemoveAll(c => c.Timestamp < cutoff);
            await SaveCommandsAsync(commands);

            return learnerCommands;
        }

        public static async Task<List<Response>> GetResponsesAsync()
        {
            return await LoadResponsesAsync();
        }

        public static async Task ClearAllAsync()
        {
            if (File.Exists(_learnersFile)) File.Delete(_learnersFile);
            if (File.Exists(_commandsFile)) File.Delete(_commandsFile);
            if (File.Exists(_responsesFile)) File.Delete(_responsesFile);
            if (File.Exists(_assessmentFile)) File.Delete(_assessmentFile);
        }

        // ===== Private Helpers =====
        private static async Task<List<Learner>> LoadLearnersAsync()
        {
            if (!File.Exists(_learnersFile)) return new List<Learner>();
            var json = await File.ReadAllTextAsync(_learnersFile);
            return System.Text.Json.JsonSerializer.Deserialize<List<Learner>>(json) ?? new List<Learner>();
        }

        private static async Task SaveLearnersAsync(List<Learner> learners)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(learners, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_learnersFile, json);
        }

        private static async Task<List<HubCommand>> LoadCommandsAsync()
        {
            if (!File.Exists(_commandsFile)) return new List<HubCommand>();
            var json = await File.ReadAllTextAsync(_commandsFile);
            return System.Text.Json.JsonSerializer.Deserialize<List<HubCommand>>(json) ?? new List<HubCommand>();
        }

        private static async Task SaveCommandsAsync(List<HubCommand> commands)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(commands, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_commandsFile, json);
        }

        private static async Task AddCommandAsync(HubCommand command)
        {
            var commands = await LoadCommandsAsync();
            commands.Add(command);
            await SaveCommandsAsync(commands);
        }

        private static async Task<List<Response>> LoadResponsesAsync()
        {
            if (!File.Exists(_responsesFile)) return new List<Response>();
            var json = await File.ReadAllTextAsync(_responsesFile);
            return System.Text.Json.JsonSerializer.Deserialize<List<Response>>(json) ?? new List<Response>();
        }

        private static async Task SaveResponsesAsync(List<Response> responses)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(responses, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_responsesFile, json);
        }
    }

    public class HubCommand
    {
        public string Type { get; set; } = string.Empty;
        public string TargetLearnerId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
