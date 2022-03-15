using Microsoft.AspNetCore.SignalR;
using ModelManagementAssignment2.Models;
using System.Threading.Tasks;

namespace ModelManagementAssignment2.Hubs
{
    public class ExpenseHub : Hub<IExpensesHub>
    {
        public async Task NewExpenseCreated(Expense newExpense)
        {
            await Clients.All.NewExpenseCreated(newExpense.Text);
        }
    }
}
