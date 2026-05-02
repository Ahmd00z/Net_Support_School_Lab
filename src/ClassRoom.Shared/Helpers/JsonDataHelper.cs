using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ClassRoom.Shared.Models;

namespace ClassRoom.Shared.Helpers
{
    public static class JsonDataHelper
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.Preserve
        };

        public static async Task SaveAssessmentAsync(Assessment assessment, string filePath)
        {
            var json = JsonSerializer.Serialize(assessment, _options);
            await File.WriteAllTextAsync(filePath, json);
        }

        public static async Task<Assessment?> LoadAssessmentAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<Assessment>(json, _options);
        }

        public static async Task SaveReportAsync(Report report, string filePath)
        {
            var json = JsonSerializer.Serialize(report, _options);
            await File.WriteAllTextAsync(filePath, json);
        }

        public static async Task<Report?> LoadReportAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<Report>(json, _options);
        }

        public static async Task SaveResponsesAsync(List<Response> responses, string filePath)
        {
            var json = JsonSerializer.Serialize(responses, _options);
            await File.WriteAllTextAsync(filePath, json);
        }

        public static async Task<List<Response>> LoadResponsesAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Response>();

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<Response>>(json, _options) ?? new List<Response>();
        }

        public static string GenerateAssessmentFileName(string title)
        {
            var safeTitle = string.Join("_", title.Split(Path.GetInvalidFileNameChars()));
            return $"{safeTitle}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        }

        public static string GenerateReportFileName(string learnerName, string assessmentTitle)
        {
            var safeLearner = string.Join("_", learnerName.Split(Path.GetInvalidFileNameChars()));
            var safeTitle = string.Join("_", assessmentTitle.Split(Path.GetInvalidFileNameChars()));
            return $"Report_{safeLearner}_{safeTitle}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        }
    }
}
