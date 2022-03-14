using Microsoft.AspNetCore.SignalR;
using ModelManagementAssignment2.Models;
using System.Threading.Tasks;

namespace ModelManagementAssignment2.Hubs
{
    public class ExpenseHub : Hub<INotificationHub>
    {
        public async Task NewExpenseCreated(Expense newExpense)
        {
            await Clients.All.NewObjectCreated($"New Expense has been created: {newExpense.Text}");
        }
    }
}
