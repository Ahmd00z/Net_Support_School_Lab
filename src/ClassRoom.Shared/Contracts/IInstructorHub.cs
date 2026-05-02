using System.Threading.Tasks;
using ClassRoom.Shared.Models;

namespace ClassRoom.Shared.Contracts
{
    public interface IInstructorHub
    {
        Task LearnerJoined(Learner learner);
        Task LearnerLeft(string learnerId);
        Task LearnerStatusChanged(string learnerId, LearnerStatus status);
        Task ResponseReceived(Response response);
        Task AssessmentCompleted(string learnerId, Report report);
        Task LockAcknowledged(string learnerId);
        Task UnlockAcknowledged(string learnerId);
    }
}
