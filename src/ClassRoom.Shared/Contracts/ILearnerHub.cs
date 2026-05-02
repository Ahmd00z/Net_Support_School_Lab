using System.Threading.Tasks;
using ClassRoom.Shared.Models;

namespace ClassRoom.Shared.Contracts
{
    public interface ILearnerHub
    {
        Task ReceiveLockCommand();
        Task ReceiveUnlockCommand();
        Task ReceiveAssessment(Assessment assessment);
        Task ReceiveAssessmentEnded();
        Task ReceiveMessage(string message, string type);
        Task ConnectionConfirmed(string learnerId);
    }
}
