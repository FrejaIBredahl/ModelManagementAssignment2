using System.Threading.Tasks;

namespace ModelManagementAssignment2.Hubs
{
    public interface INotificationHub
    {
        Task NewObjectCreated(string message);
    }
}
